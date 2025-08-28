using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsersService.Application.Abstractions;
using UsersService.Domain.Entities;

namespace UsersService.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _db;
        public UserRepository(UsersDbContext db) => _db = db;

        public async Task AddAsync(User user, CancellationToken ct)
            => await _db.Users.AddAsync(user, ct);
        // Guardas con UnitOfWork.SaveChangesAsync

        public async Task<User?> FindByEmailAsync(string email, CancellationToken ct)
            => await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, ct);

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, ct);

        public async Task<IReadOnlyList<User>> ListAsync(CancellationToken ct)
            => await _db.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .ToListAsync(ct);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
            => await _db.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email && u.IsActive, ct);

        public void Update(User user)
            => _db.Users.Update(user);

        public void Remove(User user)
            => _db.Users.Remove(user);
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly UsersDbContext _db;
        public UnitOfWork(UsersDbContext db) => _db = db;
        public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
