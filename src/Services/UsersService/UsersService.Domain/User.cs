using System;
using UsersService.Domain.Enums;

namespace UsersService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;

        public UserRole Role { get; set; } = UserRole.Usuario;

        public string PasswordHash { get; set; } = default!;
        public string PasswordSalt { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
