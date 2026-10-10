using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.ValueObjects;
using SafeFlow.API.Inventory.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Inventory.Infrastructure.Persistence.EFC.Repositories;

public sealed class ProductRepository(AppDbContext context) : BaseRepository<Product>(context), IProductRepository
{
    private IQueryable<ProductStock> WithStock(IQueryable<Product> products) => products
        .Select(product => new ProductStock(product,
            Context.Set<Lot>().Where(lot => lot.ProductId == product.Id)
                .Sum(lot => (long?)lot.Stock) ?? 0,
            Context.Set<Lot>().Where(lot => lot.ProductId == product.Id)
                .Sum(lot => (long?)lot.Reserved) ?? 0));

    public async Task<IReadOnlyList<ProductStock>> ListWithStockAsync(CancellationToken ct = default) =>
        await WithStock(Context.Set<Product>().AsNoTracking().OrderBy(product => product.Id)).ToListAsync(ct);

    public Task<ProductStock?> FindWithStockAsync(int productId, CancellationToken ct = default) =>
        WithStock(Context.Set<Product>().AsNoTracking().Where(product => product.Id == productId))
            .SingleOrDefaultAsync(ct);

    public Task<int> CountAsync(CancellationToken ct = default) => Context.Set<Product>().CountAsync(ct);
}
