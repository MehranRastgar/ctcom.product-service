namespace ctcom.ProductService.DTOs
{
    public class CreatedProductDto : CreateProductDto
    {
        public Guid Id { get; set; }
    }
    public class CreateProductDto
    {
        public string Title { get; set; } = string.Empty;    // Main product title
        public string Description { get; set; } = string.Empty;    // Product description
        public string Handle { get; set; } = string.Empty;    // URL handle or slug
        public bool IsPublished { get; set; } = false;    // Whether the product is live
        public DateTime? PublishedAt { get; set; }    // Date of publishing (optional)

        // Nested DTOs for related entities
        public List<CreateProductVariantDto> Variants { get; set; } = new List<CreateProductVariantDto>();
        public List<CreateProductOptionDto> Options { get; set; } = new List<CreateProductOptionDto>();
        public List<CreateProductImageDto> Images { get; set; } = new List<CreateProductImageDto>();
    }

    public class CreateProductVariantDto
    {
        public string Title { get; set; } = string.Empty;    // Variant title
        public decimal Price { get; set; }    // Price for this variant
        public int StockQuantity { get; set; }    // Stock available for this variant
        public List<CreateProductOptionValueDto> OptionValues { get; set; } = new List<CreateProductOptionValueDto>();    // Option values (e.g., Red, Large)
    }

    public class CreateProductOptionDto
    {
        public string Title { get; set; } = string.Empty;    // Option title (e.g., Color, Size)
    }

    public class CreateProductOptionValueDto
    {
        public string Value { get; set; } = string.Empty;    // Option value (e.g., Red, Large)
    }

    public class CreateProductImageDto
    {
        public string Url { get; set; } = string.Empty;    // Image URL
        public string AltText { get; set; } = string.Empty;    // Alt text for the image
    }
}
