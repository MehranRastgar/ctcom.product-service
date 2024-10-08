using ctcom.ProductService.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ctcom.ProductService.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<GetProductListDto> products, int totalRecords)> GetProductsAsync(int page, int pageSize, string? filter, CancellationToken cancellationToken);
        Task<GetProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> CreateProductAsync(CreateProductDto productDto, CancellationToken cancellationToken);
        Task UpdateProductAsync(UpdateProductDto productDto, CancellationToken cancellationToken);
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken);
        Task<List<string>> UploadProductImagesAsync(Guid productId, List<IFormFile> files, CancellationToken cancellationToken);
    }
}
