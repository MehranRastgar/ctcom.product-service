using AutoMapper;
using ctcom.ProductService.DTOs;
using ctcom.ProductService.Repositories;
using ctcom.ProductService.Models;
using ctcom.ProductService.Events;
using ctcom.ProductService.Messaging;

namespace ctcom.ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;

        public ProductService(IProductRepository productRepository, IMapper mapper, IMessageProducer messageProducer)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _messageProducer = messageProducer;

        }

        private bool IsImage(IFormFile file)
        {
            // List of allowed image file extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }

        private bool IsValidFileSize(IFormFile file, long maxSizeInBytes)
        {
            return file.Length <= maxSizeInBytes;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task CreateProductAsync(ProductDto productDto)
        {
            productDto.CreatedAt = DateTime.UtcNow; // Set the CreatedAt timestamp
            productDto.UpdatedAt = DateTime.UtcNow; // Set the UpdatedAt timestamp

            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product);

            // Publish ProductCreatedEvent
            var productCreatedEvent = new ProductCreatedEvent(productDto);
            await _messageProducer.PublishAsync(productCreatedEvent);
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            productDto.UpdatedAt = DateTime.UtcNow; // Update the UpdatedAt timestamp

            var product = _mapper.Map<Product>(productDto);
            await _productRepository.UpdateAsync(product);

            // Publish ProductUpdatedEvent
            var productUpdatedEvent = new ProductUpdatedEvent(productDto);
            await _messageProducer.PublishAsync(productUpdatedEvent);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _productRepository.DeleteAsync(id);

            // Publish ProductDeletedEvent
            var productDeletedEvent = new ProductDeletedEvent(id);
            await _messageProducer.PublishAsync(productDeletedEvent);
        }

        public async Task<List<string>> UploadProductImagesAsync(Guid productId, List<IFormFile> files)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException("Product not found");

            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                // Validate and store each file
                if (IsImage(file) && IsValidFileSize(file, 5 * 1024 * 1024)) // 5MB limit
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine("wwwroot", "uploads", "products", productId.ToString(), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relativeUrl = $"/uploads/products/{productId}/{fileName}";
                    imageUrls.Add(relativeUrl);

                    // Add image record to the product entity
                    product.Images.Add(new ProductImage
                    {
                        Url = relativeUrl,
                        AltText = Path.GetFileNameWithoutExtension(fileName)
                    });
                }
            }

            // Update the product to save associated images
            await _productRepository.UpdateAsync(product);

            return imageUrls;
        }

        public async Task<(IEnumerable<ProductDto>, int)> GetProductsAsync(int page, int pageSize, string? filter)
        {
            // Call repository to fetch paginated and filtered products
            var (products, totalRecords) = await _productRepository.GetProductsAsync(page, pageSize, filter);

            // Map products to DTOs
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Return the products with the total count for pagination
            return (productDtos, totalRecords);
        }
    }
}
