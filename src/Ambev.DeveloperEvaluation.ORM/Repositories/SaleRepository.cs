using System.Linq.Expressions;
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

        if (filter.MinDate is { } minDate)
        {
            var min = AsUtc(minDate);
            query = query.Where(s => s.SaleDate >= min);
        }

        if (filter.MaxDate is { } maxDate)
        {
            var max = AsUtc(maxDate);
            query = query.Where(s => s.SaleDate <= max);
        }

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

    /// <summary>
    /// Applies a possibly multi-field ordering expression such as
    /// <c>"totalAmount desc, saleNumber asc"</c>. Unknown fields fall back to the sale date;
    /// direction defaults to ascending, and with no expression the list is newest-first.
    /// </summary>
    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderByDescending(s => s.SaleDate);

        IOrderedQueryable<Sale>? ordered = null;

        foreach (var segment in order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var tokens = segment.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var descending = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            ordered = tokens[0].ToLowerInvariant() switch
            {
                "salenumber" => AddSort(ordered, query, s => s.SaleNumber, descending),
                "totalamount" => AddSort(ordered, query, s => s.TotalAmount, descending),
                "createdat" => AddSort(ordered, query, s => s.CreatedAt, descending),
                "customername" => AddSort(ordered, query, s => s.CustomerName, descending),
                "branchname" => AddSort(ordered, query, s => s.BranchName, descending),
                _ => AddSort(ordered, query, s => s.SaleDate, descending),
            };
        }

        return ordered!;
    }

    /// <summary>
    /// Normalizes a date filter to UTC. PostgreSQL <c>timestamp with time zone</c> columns
    /// only accept UTC through Npgsql; a bare date from the query string comes in as
    /// <see cref="DateTimeKind.Unspecified"/> and is treated as already being UTC.
    /// </summary>
    private static DateTime AsUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    private static IOrderedQueryable<Sale> AddSort<TKey>(
        IOrderedQueryable<Sale>? ordered,
        IQueryable<Sale> query,
        Expression<Func<Sale, TKey>> keySelector,
        bool descending)
    {
        if (ordered is null)
            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);

        return descending ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector);
    }
}
