using System.Net;

namespace BuildingBlocks.Errors;

public abstract class AppException : Exception
{
    public string Code { get; }
    public HttpStatusCode Status { get; }

    protected AppException(string code, string message, HttpStatusCode status, Exception? inner = null)
        : base(message, inner)
    {
        Code = code;
        Status = status;
    }
}

public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string code, string message)
        : base(code, message, HttpStatusCode.Unauthorized) { }
}

public sealed class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string code, string message)
        : base(code, message, HttpStatusCode.Forbidden) { }
}

public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException(string code, string message)
        : base(code, message, HttpStatusCode.NotFound) { }
}

public sealed class ConflictAppException : AppException
{
    public ConflictAppException(string code, string message)
        : base(code, message, HttpStatusCode.Conflict) { }
}

public sealed class ValidationAppException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationAppException(IDictionary<string, string[]> errors, string message = "Validation failed")
        : base(ErrorCode.ValidationFailed, message, HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }
}

public sealed class DomainRuleAppException : AppException
{
    public DomainRuleAppException(string message)
        : base(ErrorCode.DomainRuleViolated, message, HttpStatusCode.UnprocessableEntity) { }
}
