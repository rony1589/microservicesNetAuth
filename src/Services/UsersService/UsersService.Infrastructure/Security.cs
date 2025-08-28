using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuildingBlocks.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UsersService.Application.Abstractions;
using UsersService.Domain.Entities;
using UsersService.Domain.Enums;

namespace UsersService.Infrastructure
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password, out string salt)
        {
            salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly SymmetricSecurityKey _key;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly int _lifetimeHours;

        public JwtTokenFactory(IOptions<JwtOptions> options)
        {
            var o = options?.Value ?? throw new InvalidOperationException("JwtOptions no configurado.");

            var key = o.SigningKey;
            ArgumentException.ThrowIfNullOrWhiteSpace(key, "Jwt:SigningKey");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            _issuer = o.Issuer;
            _audience = o.Audience;
            _lifetimeHours = o.LifetimeHours > 0 ? o.LifetimeHours : 2;
        }

        public string Create(User user, TimeSpan? lifetime = null)
        {
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var expires = now.Add(lifetime ?? TimeSpan.FromHours(_lifetimeHours));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier,    user.Id.ToString()),
                new Claim(ClaimTypes.Email,             user.Email),
                new Claim(ClaimTypes.Name,              user.Name),
                new Claim(ClaimTypes.Role,              user.Role.ToString()) // ← enum a string
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
