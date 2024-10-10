using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using ctcom.ProductService.Controllers;
using ctcom.ProductService.DTOs;
using ctcom.ProductService.Models;
using ctcom.ProductService.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using ctcom.ProductService.Mapping;
// using Microsoft.Extensions.Logging;

namespace ctcom.ProductService.Tests.Integration
{
    public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly IMapper _mapper;
        private readonly ITestOutputHelper _output;


        public ProductControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _output = output;

            // Initialize the mock and mapper in the constructor
            _productServiceMock = new Mock<IProductService>();

            // Configure AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductMappingProfile>(); // Add the mapping profile for Product
            });
            _mapper = mapperConfig.CreateMapper();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Inject the mocked IProductService and IMapper into the service collection
                    services.AddScoped<IProductService>(_ => _productServiceMock.Object);
                    services.AddSingleton(_mapper);
                });
            }).CreateClient();
        }

        // DTO for the response
        public class ProductListResponse
        {
            public List<GetProductListDto> Data { get; set; }
            public int Total { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

        // Test: Get all products
        [Fact]
        public async Task GetProducts_ShouldReturnProductList()
        {
            // Arrange
            var expectedProducts = new List<GetProductListDto>
                            {
                                new GetProductListDto
                                {
                                    Id = Guid.NewGuid(),
                                    Title = "Product 1",
                                    Description = "Description 1",
                                    Handle = "handle-1",
                                    IsPublished = true,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                },
                                new GetProductListDto
                                {
                                    Id = Guid.NewGuid(),
                                    Title = "Product 2",
                                    Description = "Description 2",
                                    Handle = "handle-2",
                                    IsPublished = false,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                }
                            };

            int totalRecords = 21; // Total number of products

            _productServiceMock.Setup(s => s.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedProducts, totalRecords));

            // Act
            var response = await _client.GetAsync("/api/Product?page=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize the response into a concrete type
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductListResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert that the result is not null
            result.Should().NotBeNull();

            // Check the pagination information
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.Total.Should().Be(totalRecords);

            // Check that the product list count matches the expected count
            result.Data.Count.Should().Be(expectedProducts.Count);

            // Optionally: Assert specific product details
            result.Data[0].Title.Should().Be(expectedProducts[0].Title);
            result.Data[1].Description.Should().Be(expectedProducts[1].Description);
        }
        // Test: Get a single product by Id
        [Fact]
        public async Task GetProductById_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Define the product that should be returned by the service
            var expectedProduct = new GetProductDto
            {
                Id = productId,
                Title = "Test Product",
                Description = "This is a test product",
                IsPublished = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PublishedAt = DateTime.UtcNow
            };

            // Mock the GetProductByIdAsync method to return the expected product
            _productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProduct); // Simulate finding the product

            // Act
            var response = await _client.GetAsync($"/api/Product/{productId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK); // Ensure status is 200 OK

            // Read and deserialize the response content
            var content = await response.Content.ReadAsStringAsync();
            var actualProduct = JsonSerializer.Deserialize<GetProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Check that the returned product matches the expected product
            actualProduct.Should().NotBeNull();
            actualProduct.Id.Should().Be(expectedProduct.Id);
            actualProduct.Title.Should().Be(expectedProduct.Title);
            actualProduct.Description.Should().Be(expectedProduct.Description);
        }

        // Test: Create a product
        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Title = "string",
                Description = "string",
                Handle = "string",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow
            };

            var createdProductDto = new CreatedProductDto
            {
                Id = Guid.NewGuid(),
                Title = "string",
                Description = "string",
                Handle = "string",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow
            };

            _productServiceMock.Setup(s => s.CreateProductAsync(It.IsAny<CreateProductDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdProductDto);

            var content = new StringContent(JsonSerializer.Serialize(createProductDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Product", content);

            // Log the Location header for debugging
            var locationHeader = response.Headers.Location?.ToString() ?? "No Location Header";
            _output.WriteLine($"Location header returned from API: {locationHeader}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created); // Ensure status is 201 Created
            var responseBody = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response Body: {responseBody}");

            // Check that the location header contains the expected product ID
            locationHeader.Should().Contain(createdProductDto.Id.ToString());
        }
        // Test: Update a product
        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto
            {
                Id = productId,
                Title = "Updated Product",
                Description = "Updated Description",
                IsPublished = true
            };

            _productServiceMock.Setup(s => s.UpdateProductAsync(updateProductDto, default))
                .Returns(Task.CompletedTask);

            var content = new StringContent(JsonSerializer.Serialize(updateProductDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/Product/{productId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        // Test: Delete a product
        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Mock the GetProductByIdAsync to return a product, ensuring the product exists for deletion
            var productDto = new GetProductDto
            {
                Id = productId,
                Title = "Test Product",
                Description = "Test Description",
                IsPublished = true
            };

            _productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto); // Simulate finding the product

            // Mock the DeleteProductAsync to simulate successful deletion
            _productServiceMock.Setup(s => s.DeleteProductAsync(productId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _client.DeleteAsync($"/api/Product/{productId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent); // Ensure status is 204 No Content
        }

        // Test: Upload product images
        [Fact]
        public async Task UploadProductImages_ShouldReturnImageUrls()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var mockProduct = new GetProductDto { Id = productId, Title = "Test Product" };
            var expectedImageUrls = new List<string>
                {
                    "/uploads/products/product1/image1.jpg",
                    "/uploads/products/product1/image2.jpg"
                };

            var files = new List<IFormFile>
                {
                    CreateTestFormFile("image1.jpg"),
                    CreateTestFormFile("image2.jpg")
                };

            // Mock the service call for checking if the product exists
            _productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockProduct);

            // Mock the service call for uploading images
            _productServiceMock.Setup(s => s.UploadProductImagesAsync(productId, It.IsAny<List<IFormFile>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedImageUrls);

            // Create MultipartFormDataContent with the images
            var content = new MultipartFormDataContent();
            foreach (var file in files)
            {
                content.Add(new StreamContent(file.OpenReadStream()), "files", file.FileName);
            }

            // Act
            var response = await _client.PostAsync($"/api/Product/{productId}/upload-images", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize the response content
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert that the result contains the expected image URLs
            result.Should().NotBeNull();
            result["imageUrls"].Should().BeEquivalentTo(expectedImageUrls);
        }

        // Helper method to create a test IFormFile
        private IFormFile CreateTestFormFile(string fileName)
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Fake image content";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            fileMock.Setup(f => f.OpenReadStream()).Returns(fileStream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(fileStream.Length);

            return fileMock.Object;
        }


    }
}
