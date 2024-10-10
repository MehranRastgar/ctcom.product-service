using AutoMapper;
using ctcom.ProductService.DTOs;
using ctcom.ProductService.Repositories;
using ctcom.ProductService.Models;
using ctcom.ProductService.Events;
using ctcom.ProductService.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<(IEnumerable<GetProductListDto> products, int totalRecords)> GetProductsAsync(int page, int pageSize, string? filter, CancellationToken cancellationToken)
        {
            var (products, totalRecords) = await _productRepository.GetProductsAsync(page, pageSize, filter, cancellationToken);
            var productDtos = _mapper.Map<IEnumerable<GetProductListDto>>(products);
            return (productDtos, totalRecords);
        }

        public async Task<GetProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            return product == null ? null : _mapper.Map<GetProductDto>(product);
        }

        // public async Task CreateProductAsync(CreateProductDto productDto, CancellationToken cancellationToken)
        // {
        //     var product = _mapper.Map<Product>(productDto);
        //     await _productRepository.AddAsync(product, cancellationToken);

        //     var productCreatedEvent = new ProductCreatedEvent(product);
        //     await _messageProducer.PublishAsync(productCreatedEvent);
        // }
        // public async Task<CreatedProductDto> CreateProductAsync(CreateProductDto productDto, CancellationToken cancellationToken)
        // {
        //     var product = _mapper.Map<Product>(productDto);
        //     product.CreatedAt = DateTime.UtcNow;
        //     product.UpdatedAt = DateTime.UtcNow;

        //     // Call the repository and return the generated Id
        //     Guid createdProductId = await _productRepository.AddAsync(product, cancellationToken);
        //     product.Id = createdProductId;
        //     var createdProductDto = _mapper.Map<CreatedProductDto>(product);
        //     return createdProductDto;
        // }
        public async Task<CreatedProductDto> CreateProductAsync(CreateProductDto productDto, CancellationToken cancellationToken)
        {
            // Log to ensure the method was called with valid data
            Console.WriteLine("Service Layer: CreateProductAsync called");

            if (productDto == null)
            {
                Console.WriteLine("Error: productDto is null");
                throw new ArgumentNullException(nameof(productDto), "productDto is null");
            }

            // Log the productDto properties
            Console.WriteLine($"productDto: Title={productDto.Title}, Description={productDto.Description}");

            // Map DTO to Product model
            var product = _mapper.Map<Product>(productDto);

            // Ensure the mapping was successful
            if (product == null)
            {
                Console.WriteLine("Error: Mapping from CreateProductDto to Product failed");
                throw new Exception("Mapping failed: CreateProductDto to Product");
            }

            // Log the mapped product details
            Console.WriteLine($"Mapped Product: Title={product.Title}, Description={product.Description}");

            // Attempt to save the product to the database
            try
            {
                var createdProductId = await _productRepository.AddAsync(product, cancellationToken);

                if (createdProductId == Guid.Empty)
                {
                    Console.WriteLine("Error: Failed to generate product ID");
                    throw new Exception("Product creation failed: No ID generated");
                }

                Console.WriteLine($"Product created successfully with ID: {createdProductId}");

                // Map the created product back to DTO
                var createdProductDto = _mapper.Map<CreatedProductDto>(product);

                if (createdProductDto == null)
                {
                    Console.WriteLine("Error: Mapping from Product to CreatedProductDto failed");
                    throw new Exception("Mapping failed: Product to CreatedProductDto");
                }

                createdProductDto.Id = createdProductId;
                return createdProductDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during product creation: {ex.Message}");
                throw;
            }
        }


        public async Task UpdateProductAsync(UpdateProductDto productDto, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.UpdateAsync(product, cancellationToken);

            var productUpdatedEvent = new ProductUpdatedEvent(product);
            await _messageProducer.PublishAsync(productUpdatedEvent);
        }

        public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            await _productRepository.DeleteAsync(id, cancellationToken);

            var productDeletedEvent = new ProductDeletedEvent(id);
            await _messageProducer.PublishAsync(productDeletedEvent);
        }

        public async Task<List<string>> UploadProductImagesAsync(Guid productId, List<IFormFile> files, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException("Product not found");

            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (IsImage(file) && IsValidFileSize(file, 5 * 1024 * 1024))
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine("wwwroot", "uploads", "products", productId.ToString(), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream, cancellationToken);
                    }

                    var relativeUrl = $"/uploads/products/{productId}/{fileName}";
                    imageUrls.Add(relativeUrl);

                    product.Images.Add(new ProductImage
                    {
                        Url = relativeUrl,
                        AltText = Path.GetFileNameWithoutExtension(fileName)
                    });
                }
            }

            await _productRepository.UpdateAsync(product, cancellationToken);

            return imageUrls;
        }

        private bool IsImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private bool IsValidFileSize(IFormFile file, long maxSizeInBytes)
        {
            return file.Length <= maxSizeInBytes;
        }
    }
}
