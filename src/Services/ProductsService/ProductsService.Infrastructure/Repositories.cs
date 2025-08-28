using Microsoft.EntityFrameworkCore;
using ProductsService.Application.Abstractions;
using ProductsService.Domain.Entities;

namespace ProductsService.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _db;
    public ProductRepository(ProductsDbContext db) => _db = db;

    public async Task AddAsync(Product p, CancellationToken ct) => await _db.Products.AddAsync(p, ct);
    public Task<Product?> GetAsync(Guid id, CancellationToken ct) => _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IEnumerable<Product>> ListAsync(CancellationToken ct) => await _db.Products.AsNoTracking().ToListAsync(ct);
    public void Remove(Product p) => _db.Products.Remove(p);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ProductsDbContext _db;
    public UnitOfWork(ProductsDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
