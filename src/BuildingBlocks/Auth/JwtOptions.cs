namespace BuildingBlocks.Auth;

public sealed class JwtOptions
{
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public string? SigningKey { get; init; }
    // opcional; default 2 horas si no lo pones en appsettings
    public int LifetimeHours { get; init; } = 2;
}
