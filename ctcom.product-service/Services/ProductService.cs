using ctcom.ProductService.Events;
using ctcom.ProductService.Repositories;
using ctcom.ProductService.Messaging;
using ctcom.ProductService.Services;
using ctcom.ProductService.Models;

namespace ctcom.ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMessageProducer _messageProducer;

        public ProductService(IProductRepository productRepository, IMessageProducer messageProducer)
        {
            _productRepository = productRepository;
            _messageProducer = messageProducer;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            var productCreatedEvent = new ProductCreatedEvent(product.Id, product.Name, product.Price);
            await _messageProducer.PublishAsync(productCreatedEvent);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
            var productUpdatedEvent = new ProductUpdatedEvent(product.Id, product.Name, product.Price);
            await _messageProducer.PublishAsync(productUpdatedEvent);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _productRepository.DeleteAsync(id);
            var productDeletedEvent = new ProductDeletedEvent(id);
            await _messageProducer.PublishAsync(productDeletedEvent);
        }
    }
}
