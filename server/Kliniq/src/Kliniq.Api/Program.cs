using Kliniq.Api.Binders;
using Kliniq.Api.Extensions;
using Kliniq.Api.OpenApi;
using Kliniq.Application;
using Kliniq.Application.Common.Settings;
using Kliniq.Application.Common.Interfaces;
using Kliniq.Infrastructure;
using Kliniq.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Text;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting KLINIQ API");
    var builder = WebApplication.CreateBuilder(args);

    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    ValidateProductionConfiguration(builder.Configuration, builder.Environment);

    builder.Services.AddOpenApi(options =>
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

    builder.Services.AddControllers(options =>
        options.ModelBinderProviders.Insert(0, new FormWithFilesModelBinderProvider()));

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    builder.Services.Configure<SymptomMatchingOptions>(builder.Configuration.GetSection(SymptomMatchingOptions.SectionName));
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var dataProtection = builder.Services.AddDataProtection()
        .SetApplicationName("KLINIQ");
    var dataProtectionKeysPath = builder.Configuration["DataProtection:KeysPath"];
    if (!string.IsNullOrWhiteSpace(dataProtectionKeysPath))
    {
        var fullKeysPath = Path.GetFullPath(dataProtectionKeysPath);
        Directory.CreateDirectory(fullKeysPath);
        dataProtection.PersistKeysToFileSystem(new DirectoryInfo(fullKeysPath));
    }

    var jwtKey = builder.Configuration["JwtSettings:Key"]
        ?? throw new InvalidOperationException("JwtSettings:Key is required.");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrWhiteSpace(token)) context.Token = token;
                return Task.CompletedTask;
            }
        };
    });

    var configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
        .ToArray() ?? [];

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
        {
            var origins = configuredOrigins.Length > 0
                ? configuredOrigins
                : builder.Environment.IsDevelopment()
                    ? ["http://localhost:5173", "https://localhost:5173"]
                    : throw new InvalidOperationException("Cors:AllowedOrigins must be configured in production.");

            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddAuthorization();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddHealthChecks();

    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });
    builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
    builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.AddPolicy("auth", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
        options.AddPolicy("symptom-search", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    });

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

    var app = builder.Build();

    // Force the embedded symptom catalog to load and validate during startup.
    _ = app.Services.GetRequiredService<ISymptomAnalysisService>();

    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
        await seeder.SeedAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "KLINIQ API";
            options.DarkMode = true;
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.Layout = ScalarLayout.Modern;
            options.ShowSidebar = true;
            options.AddPreferredSecuritySchemes("Bearer")
                .AddHttpAuthentication("Bearer", auth => auth.Token = string.Empty);
        });
    }
    else
    {
        app.UseHsts();
    }

    app.UseForwardedHeaders();
    app.UseHttpsRedirection();
    app.UseResponseCompression();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
        };
    });

    app.UseExceptionHandler();
    app.UseCors("Frontend");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health");
    app.MapControllers();

    app.Run();
}
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "KLINIQ API failed to start.");
}
finally
{
    Log.CloseAndFlush();
}

static void ValidateProductionConfiguration(IConfiguration configuration, IWebHostEnvironment environment)
{
    if (environment.IsDevelopment()) return;

    var required = new[]
    {
        "ConnectionStrings:DefaultConnection",
        "JwtSettings:Key",
        "JwtSettings:Issuer",
        "JwtSettings:Audience",
        "FileStorage:BasePath",
        "DataProtection:KeysPath",
        "App:BaseUrl",
        "App:TimeZoneId",
        "SmtpSettings:Host",
        "SmtpSettings:FromEmail",
        "SmtpSettings:Username",
        "SmtpSettings:Password"
    };

    foreach (var key in required)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value) || value.Contains("your-", StringComparison.OrdinalIgnoreCase) || value.Contains("server_name", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Production configuration '{key}' is missing or still contains a placeholder value.");
    }
}
