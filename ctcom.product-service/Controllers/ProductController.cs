using Microsoft.AspNetCore.Mvc;
using ctcom.ProductService.Services;
using ctcom.ProductService.DTOs;

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

        // [HttpGet]
        // public async Task<IActionResult> GetAllProducts()
        // {
        //     var products = await _productService.GetAllProductsAsync();
        //     return Ok(products);
        // }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {
            var (products, totalRecords) = await _productService.GetProductsAsync(page, pageSize, filter);

            // Return paginated and filtered data
            return Ok(new
            {
                data = products,
                total = totalRecords,
                page,
                pageSize
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductDto productDto)
        {

            if (id != productDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productService.UpdateProductAsync(productDto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("{id:guid}/upload-images")]
        public async Task<IActionResult> UploadProductImages(Guid id, [FromForm] List<IFormFile> files)
        {
            // Validate if the product exists
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            // Validate if any files are uploaded
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            // Upload each image and associate it with the product
            var imageUrls = await _productService.UploadProductImagesAsync(id, files);

            // Return the URLs of the uploaded images
            return Ok(new { ImageUrls = imageUrls });
        }

    }
}
