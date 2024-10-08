namespace ctcom.ProductService.DTOs
{
    public class GetProductDto
    {
        public Guid Id { get; set; }    // Product ID
        public string Title { get; set; } = string.Empty;    // Main product title
        public string Description { get; set; } = string.Empty;    // Product description
        public string Handle { get; set; } = string.Empty;    // URL handle or slug
        public bool IsPublished { get; set; } = false;    // Whether the product is live
        public DateTime? PublishedAt { get; set; }    // Date of publishing (optional)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    // Creation timestamp
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;    // Last update timestamp

        // Nested DTOs for related entities
        public List<GetProductVariantDto> Variants { get; set; } = new List<GetProductVariantDto>();
        public List<GetProductOptionDto> Options { get; set; } = new List<GetProductOptionDto>();
        public List<GetProductImageDto> Images { get; set; } = new List<GetProductImageDto>();
    }

    public class GetProductVariantDto
    {
        public Guid Id { get; set; }    // Variant ID
        public string Title { get; set; } = string.Empty;    // Variant title
        public decimal Price { get; set; }    // Price for this variant
        public int StockQuantity { get; set; }    // Stock available for this variant
        public List<GetProductOptionValueDto> OptionValues { get; set; } = new List<GetProductOptionValueDto>();    // Option values (e.g., Red, Large)
    }

    public class GetProductOptionDto
    {
        public Guid Id { get; set; }    // Option ID
        public string Title { get; set; } = string.Empty;    // Option title (e.g., Color, Size)
    }

    public class GetProductOptionValueDto
    {
        public Guid Id { get; set; }    // Option value ID
        public string Value { get; set; } = string.Empty;    // Option value (e.g., Red, Large)
    }

    public class GetProductImageDto
    {
        public Guid Id { get; set; }    // Image ID
        public string Url { get; set; } = string.Empty;    // Image URL
        public string AltText { get; set; } = string.Empty;    // Alt text for the image
    }
}
