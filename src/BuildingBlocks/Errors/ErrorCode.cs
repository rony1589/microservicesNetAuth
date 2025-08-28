namespace BuildingBlocks.Errors;

public static class ErrorCode
{
    public const string AuthInvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string AuthUnauthorized = "AUTH_UNAUTHORIZED";
    public const string AuthForbidden = "AUTH_FORBIDDEN";

    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";

    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string DomainRuleViolated = "DOMAIN_RULE_VIOLATED";

    public const string DatabaseError = "DATABASE_ERROR";
    public const string Unexpected = "UNEXPECTED_ERROR";
}
