using Microsoft.AspNetCore.Mvc;
using ctcom.ProductService.Services;
using ctcom.ProductService.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ctcom.ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET /api/Product
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null, CancellationToken cancellationToken = default)
        {
            var (products, totalRecords) = await _productService.GetProductsAsync(page, pageSize, filter, cancellationToken);
            return Ok(new { data = products, total = totalRecords, page, pageSize });
        }

        // GET /api/Product/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST /api/Product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Call the service to create the product and get the generated Id
            var productId = await _productService.CreateProductAsync(productDto, cancellationToken);

            // Return the created product's Id
            return CreatedAtAction(nameof(GetProductById), new { id = productId }, productDto);
        }

        // PUT /api/Product/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto productDto, CancellationToken cancellationToken = default)
        {
            if (id != productDto.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productService.UpdateProductAsync(productDto, cancellationToken);
            return NoContent();
        }

        // DELETE /api/Product/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
                return NotFound();

            await _productService.DeleteProductAsync(id, cancellationToken);
            return NoContent();
        }

        // POST /api/Product/{id}/upload-images
        [HttpPost("{id:guid}/upload-images")]
        public async Task<IActionResult> UploadProductImages(Guid id, [FromForm] List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            // Validate if the product exists
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
                return NotFound("Product not found.");

            // Validate if any files are uploaded
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            // Upload each image and associate it with the product
            var imageUrls = await _productService.UploadProductImagesAsync(id, files, cancellationToken);

            // Return the URLs of the uploaded images
            return Ok(new { ImageUrls = imageUrls });
        }
    }
}
