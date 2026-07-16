using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Kliniq.Api.Extensions;
using Kliniq.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Text.Json;

namespace Kliniq.Tests.Middleware
{
    public class GlobalExceptionHandlerTests
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly GlobalExceptionHandler _sut;

        public GlobalExceptionHandlerTests()
        {
            _problemDetailsService = Substitute.For<IProblemDetailsService>();
            _problemDetailsService
                .TryWriteAsync(Arg.Any<ProblemDetailsContext>())
                .Returns(callInfo =>
                {
                    var ctx = callInfo.Arg<ProblemDetailsContext>();
                    return WriteJsonAsync(ctx.HttpContext, ctx.ProblemDetails);
                });

            _sut = new GlobalExceptionHandler(
                NullLogger<GlobalExceptionHandler>.Instance,
                _problemDetailsService);
        }

        private static async ValueTask<bool> WriteJsonAsync(HttpContext ctx, ProblemDetails pd)
        {
            ctx.Response.ContentType = "application/problem+json";
            await JsonSerializer.SerializeAsync(
                ctx.Response.Body,
                new
                {
                    title = pd.Title,
                    detail = pd.Detail,
                    status = pd.Status,
                    extensions = pd.Extensions
                });
            return true;
        }

        private static DefaultHttpContext BuildHttpContext()
        {
            var ctx = new DefaultHttpContext();
            ctx.Response.Body = new MemoryStream();
            return ctx;
        }

        private static async Task<JsonDocument> ReadResponseBody(HttpContext ctx)
        {
            ctx.Response.Body.Seek(0, SeekOrigin.Begin);
            return await JsonDocument.ParseAsync(ctx.Response.Body);
        }

        [Fact]
        public async Task Handle_ValidationException_Returns400()
        {
            var ctx = BuildHttpContext();
            var failures = new List<ValidationFailure>
            {
                new("Email",    "Email is required."),
                new("Password", "Password is required.")
            };
            var exception = new ValidationException(failures);

            var handled = await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            handled.Should().BeTrue();
            ctx.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var body = await ReadResponseBody(ctx);
            body.RootElement.GetProperty("title").GetString().Should().Be("Validation Failed");
            body.RootElement.TryGetProperty("extensions", out _).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ValidationException_IncludesFieldErrors()
        {
            var ctx = BuildHttpContext();
            var failures = new List<ValidationFailure>
            {
                new("Email", "Email is required."),
                new("Email", "Invalid email format.")
            };
            var exception = new ValidationException(failures);

            await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            var body = await ReadResponseBody(ctx);
            body.RootElement.GetProperty("extensions")
                .TryGetProperty("errors", out _).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_DomainException_Returns422()
        {
            var ctx = BuildHttpContext();
            var exception = new DomainException("Request already processed");

            var handled = await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            handled.Should().BeTrue();
            ctx.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);

            var body = await ReadResponseBody(ctx);
            body.RootElement.GetProperty("title").GetString().Should().Be("Business Rule Violation");
            body.RootElement.GetProperty("detail").GetString().Should().Be("Request already processed");
        }

        [Fact]
        public async Task Handle_InvalidOperationException_Returns409()
        {
            var ctx = BuildHttpContext();
            var exception = new InvalidOperationException("A pending request with the same email already exists.");

            var handled = await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            handled.Should().BeTrue();
            ctx.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);

            var body = await ReadResponseBody(ctx);
            body.RootElement.GetProperty("title").GetString().Should().Be("Operation Not Allowed");
        }

        [Fact]
        public async Task Handle_UnauthorizedAccessException_Returns401()
        {
            var ctx = BuildHttpContext();
            var exception = new UnauthorizedAccessException("Invalid email or password");

            var handled = await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            handled.Should().BeTrue();
            ctx.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);

            var body = await ReadResponseBody(ctx);
            body.RootElement.GetProperty("title").GetString().Should().Be("Unauthorized");
        }

        [Fact]
        public async Task Handle_KeyNotFoundException_Returns404()
        {
            var ctx = BuildHttpContext();
            var exception = new KeyNotFoundException("Resource not found");

            var handled = await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            handled.Should().BeTrue();
            ctx.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task Handle_ArgumentException_Returns400()
        {
            var ctx = BuildHttpContext();
            var exception = new ArgumentException("Street is required");

            var handled = await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            handled.Should().BeTrue();
            ctx.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Handle_UnknownException_Returns500()
        {
            var ctx = BuildHttpContext();
            var exception = new Exception("Something went very wrong");

            var handled = await _sut.TryHandleAsync(ctx, exception, CancellationToken.None);

            handled.Should().BeTrue();
            ctx.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            var body = await ReadResponseBody(ctx);
            body.RootElement.GetProperty("title").GetString()
                .Should().Be("An unexpected error occurred.");
        }

        [Fact]
        public async Task Handle_AlwaysReturnsTrue()
        {
            var ctx = BuildHttpContext();
            var result = await _sut.TryHandleAsync(ctx, new Exception("any"), CancellationToken.None);
            result.Should().BeTrue();
        }
    }
}