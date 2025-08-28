using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers_Bcrypt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            const string AdminPlain = "Admin123";
            const string UserPlain = "User123";

            var adminSalt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var adminHash = BCrypt.Net.BCrypt.HashPassword(AdminPlain, adminSalt);

            var userSalt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var userHash = BCrypt.Net.BCrypt.HashPassword(UserPlain, userSalt);

            var adminId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            static string Esc(string s) => s.Replace("'", "''");

            migrationBuilder.Sql($@"
            INSERT INTO users (id, email, name, role, password_hash, password_salt, is_active, created_at)
            VALUES
             ('{adminId}', 'admin@demo.com', 'Admin',   'Admin',   '{Esc(adminHash)}', '{Esc(adminSalt)}', TRUE, now()),
             ('{userId}',  'user@demo.com',  'Usuario', 'Usuario', '{Esc(userHash)}',  '{Esc(userSalt)}',  TRUE, now())
            ON CONFLICT (email) DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM users WHERE email IN ('admin@demo.com','user@demo.com');");
        }
    }
}
