using ctcom.ProductService.Models;

namespace ctcom.ProductService.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<(IEnumerable<Product>, int)> GetProductsAsync(int page, int pageSize, string? filter);
    }
}
