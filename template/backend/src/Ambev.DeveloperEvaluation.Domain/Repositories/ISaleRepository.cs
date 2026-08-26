using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="Sale"/> aggregate operations.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Persists a new sale (including its items).
    /// </summary>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its identifier, including its items. The returned instance is tracked.
    /// </summary>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its sale number, or <c>null</c> when it does not exist.
    /// </summary>
    Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a page of sales matching the supplied filter, together with the total count.
    /// </summary>
    Task<(IReadOnlyList<Sale> Items, int TotalCount)> ListAsync(SaleListFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists the changes made to a tracked sale.
    /// </summary>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale by its identifier.
    /// </summary>
    /// <returns><c>true</c> when the sale existed and was removed; otherwise <c>false</c>.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
