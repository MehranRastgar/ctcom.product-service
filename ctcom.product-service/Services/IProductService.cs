using ctcom.ProductService.DTOs;
using ctcom.ProductService.Models;

namespace ctcom.ProductService.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task CreateProductAsync(ProductDto product);
        Task UpdateProductAsync(ProductDto product);
        Task DeleteProductAsync(Guid id);
        Task<List<string>> UploadProductImagesAsync(Guid productId, List<IFormFile> files);
        Task<(IEnumerable<ProductDto>, int)> GetProductsAsync(int page, int pageSize, string? filter);
    }
}
