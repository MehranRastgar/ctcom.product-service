using Xunit;
using Moq;
using AutoMapper;
using ctcom.ProductService.Repositories;
using ctcom.ProductService.Services;
using ctcom.ProductService.Models;
using ctcom.ProductService.DTOs;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using ctcom.ProductService.Mapping;
using ctcom.ProductService.Messaging;

namespace ctcom.ProductService.Tests
{
    public class ProductServiceTests
    {
        private readonly ctcom.ProductService.Services.ProductService _productService;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IMapper _mapper;
        private readonly Mock<IMessageProducer> _mockMessageProducer;

        public ProductServiceTests()
        {
            // Mock the product repository
            _mockProductRepository = new Mock<IProductRepository>();

            // Mock the message producer
            _mockMessageProducer = new Mock<IMessageProducer>();

            // Set up AutoMapper with the ProductMappingProfile
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ProductMappingProfile()));
            _mapper = config.CreateMapper();

            // Initialize the product service with the mocked repository, mapper, and message producer
            _productService = new ctcom.ProductService.Services.ProductService(
                _mockProductRepository.Object,
                _mapper,
                _mockMessageProducer.Object
            );
        }
        [Fact]
        public async Task GetAllProducts_ReturnsProductDtos()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Title = "Product 1", Description = "Description 1" },
                new Product { Id = Guid.NewGuid(), Title = "Product 2", Description = "Description 2" }
            };
            _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Product 1", result.First().Title);
        }

        [Fact]
        public async Task GetProductById_ReturnsProductDto()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Title = "Product 1", Description = "Description 1" };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(product.Id)).ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Product 1", result.Title);
        }

        [Fact]
        public async Task CreateProduct_AddsProduct()
        {
            // Arrange
            var productDto = new ProductDto
            {
                Title = "New Product",
                Description = "New Description"
            };

            // Act
            await _productService.CreateProductAsync(productDto);

            // Assert
            _mockProductRepository.Verify(repo => repo.AddAsync(It.Is<Product>(
                p => p.Title == "New Product"
                     && p.Description == "New Description"
                     && p.CreatedAt != default(DateTime) // Ensure CreatedAt is set
                     && p.UpdatedAt != default(DateTime) // Ensure UpdatedAt is set
            )), Times.Once);
        }


        [Fact]
        public async Task DeleteProduct_RemovesProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            await _productService.DeleteProductAsync(productId);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteAsync(productId), Times.Once);
        }

    }
}
