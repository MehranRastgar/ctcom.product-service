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
    }
}
