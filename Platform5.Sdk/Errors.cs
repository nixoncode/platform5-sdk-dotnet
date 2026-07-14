namespace Platform5.Sdk;

public class Platform5Exception : Exception
{
    public int StatusCode { get; }
    public string? Errors { get; }
    public string? RequestId { get; }

    public Platform5Exception(int statusCode, string message, string? errors = null, string? requestId = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
        RequestId = requestId;
    }
}

public class UnauthorizedException : Platform5Exception
{
    public UnauthorizedException(string message, string? errors = null, string? requestId = null)
        : base(401, message, errors, requestId) { }
}

public class InsufficientBalanceException : Platform5Exception
{
    public InsufficientBalanceException(string message, string? errors = null, string? requestId = null)
        : base(402, message, errors, requestId) { }
}

public class ForbiddenException : Platform5Exception
{
    public ForbiddenException(string message, string? errors = null, string? requestId = null)
        : base(403, message, errors, requestId) { }
}

public class NotFoundException : Platform5Exception
{
    public NotFoundException(string message, string? errors = null, string? requestId = null)
        : base(404, message, errors, requestId) { }
}

public class ValidationException : Platform5Exception
{
    public ValidationException(string message, string? errors = null, string? requestId = null)
        : base(422, message, errors, requestId) { }
}

public class RateLimitException : Platform5Exception
{
    public int Limit { get; }
    public int Remaining { get; }

    public RateLimitException(string message, string? errors, string? requestId, int limit, int remaining)
        : base(429, message, errors, requestId)
    {
        Limit = limit;
        Remaining = remaining;
    }
}
