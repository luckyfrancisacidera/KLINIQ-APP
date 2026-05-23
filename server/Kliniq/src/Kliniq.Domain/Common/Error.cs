namespace Kliniq.Domain.Common
{
    public sealed record class Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
    {
        public static Error NotFound(string code, string messge) => new(code, messge, ErrorType.NotFound);
        public static Error Validation(string code, string messge) => new(code, messge, ErrorType.Validation);
        public static Error Conflict(string code, string messge) => new(code, messge, ErrorType.Conflict);
        public static Error Failure(string code, string messge) => new(code, messge, ErrorType.Failure);
        public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);
    }

    public enum ErrorType
    {
        Failure,
        Validation,
        NotFound,
        Conflict,
        Unauthorized
    }
}

