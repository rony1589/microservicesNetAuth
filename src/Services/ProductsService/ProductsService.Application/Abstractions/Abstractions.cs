using ProductsService.Domain.Entities;

namespace ProductsService.Application.Abstractions;

public interface IProductRepository
{
    Task AddAsync(Product p, CancellationToken ct);
    Task<Product?> GetAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Product>> ListAsync(CancellationToken ct);
    void Remove(Product p);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}
