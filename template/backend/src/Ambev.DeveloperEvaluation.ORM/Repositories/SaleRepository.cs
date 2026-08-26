using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of <see cref="ISaleRepository"/> using Entity Framework Core.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <inheritdoc />
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<Sale> Items, int TotalCount)> ListAsync(SaleListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CustomerId is { } customerId)
            query = query.Where(s => s.CustomerId == customerId);

        if (filter.BranchId is { } branchId)
            query = query.Where(s => s.BranchId == branchId);

        if (filter.IsCancelled is { } isCancelled)
            query = query.Where(s => s.IsCancelled == isCancelled);

        query = ApplyOrdering(query, filter.Order);

        var totalCount = await query.CountAsync(cancellationToken);

        var page = filter.Page < 1 ? 1 : filter.Page;
        var size = filter.Size < 1 ? 10 : filter.Size;

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sale is null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? order)
    {
        var descending = order?.Contains("desc", StringComparison.OrdinalIgnoreCase) ?? true;
        var field = (order ?? "saleDate").Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.ToLowerInvariant();

        return field switch
        {
            "salenumber" => descending ? query.OrderByDescending(s => s.SaleNumber) : query.OrderBy(s => s.SaleNumber),
            "totalamount" => descending ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
            "createdat" => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            _ => descending ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate),
        };
    }
}
