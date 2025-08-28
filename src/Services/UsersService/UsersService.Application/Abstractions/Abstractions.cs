using UsersService.Domain.Entities;

namespace UsersService.Application.Abstractions;

public interface IUserRepository
{
    // ← usado por GetUserByIdHandler
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    // ← usado por ListUsersHandler
    Task<IReadOnlyList<User>> ListAsync(CancellationToken ct);

    // Métodos comunes (si ya los tienes con otro nombre, puedes dejarlos;
    // estos ayudan a registro/login y administración)
    Task<User?> FindByEmailAsync(string email, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);

    Task AddAsync(User user, CancellationToken ct);
    void Update(User user);
    void Remove(User user);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}

public interface IPasswordHasher
{
    string Hash(string password, out string salt);
    bool Verify(string password, string hash);
}

public interface IJwtTokenFactory
{
    string Create(User user, TimeSpan? lifetime = null);
}
