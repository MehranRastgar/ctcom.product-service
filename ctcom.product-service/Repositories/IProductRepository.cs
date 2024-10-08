using ctcom.ProductService.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ctcom.ProductService.Repositories
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product>, int totalRecords)> GetProductsAsync(int page, int pageSize, string? filter, CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> AddAsync(Product product, CancellationToken cancellationToken);
        Task UpdateAsync(Product product, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
