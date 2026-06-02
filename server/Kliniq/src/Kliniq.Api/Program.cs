using Kliniq.Api.Binders;
using Kliniq.Api.Extensions;
using Kliniq.Api.OpenApi;
using Kliniq.Application;
using Kliniq.Application.Common.Settings;
using Kliniq.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Kliniq API");

    var builder = WebApplication.CreateBuilder(args);

    // Prevents JWT middleware from remapping "sub" → ClaimTypes.NameIdentifier
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

    // OpenAPI
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });

    // Controllers + model binder — ONE call only
    builder.Services.AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new FormWithFilesModelBinderProvider());
    });

    // Serilog
    builder.Host.UseSerilog((context, services, config) =>
        config.ReadFrom.Configuration(context.Configuration)
              .ReadFrom.Services(services)
              .Enrich.FromLogContext());

    // Layers
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // JWT
    builder.Services.AddAntiforgery();
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
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(context.Exception);
                return Task.CompletedTask;
            }
        };
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy
                .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });


    builder.Services.AddAuthorization();
    builder.Services.AddEndpointsApiExplorer();

    // Exception handling
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    //Settings
    builder.Services.Configure<AppSettings>(
        builder.Configuration.GetSection("App"));

    var app = builder.Build();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        foreach (var url in app.Urls)
        {
            Log.Information("Kliniq API running on {Url}", url);
        }

        Log.Information(
            "Kliniq API Scalar Docs running on {Url}",
            $"{app.Urls.First()}/scalar/v1");
    });

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
            options.Title = "Kliniq API";
            options.DarkMode = true;
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.HideModels = false;
            options.Layout = ScalarLayout.Modern;
            options.ShowSidebar = true;

            options.AddPreferredSecuritySchemes("Bearer")
                   .AddHttpAuthentication("Bearer", auth =>
                   {
                       auth.Token = "";
                   });
        });
    }

    if (!app.Environment.IsDevelopment())
        app.UseHttpsRedirection();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
        };

        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (ex is not null || httpContext.Response.StatusCode >= 500)
                return Serilog.Events.LogEventLevel.Error;
            if (httpContext.Response.StatusCode >= 400)
                return Serilog.Events.LogEventLevel.Warning;
            return Serilog.Events.LogEventLevel.Information;
        };
    });

    app.UseExceptionHandler();
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Kliniq API failed to start.");
}
finally
{
    Log.CloseAndFlush();
}