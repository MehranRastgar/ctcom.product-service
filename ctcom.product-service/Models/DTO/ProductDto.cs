namespace ctcom.ProductService.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    // Timestamp when product is created
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;    // Timestamp when product is last updated

        public ICollection<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
        public ICollection<ProductOptionDto> Options { get; set; } = new List<ProductOptionDto>();
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }

    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public ICollection<ProductOptionValueDto> OptionValues { get; set; } = new List<ProductOptionValueDto>();
    }

    public class ProductOptionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public ICollection<ProductOptionValueDto> OptionValues { get; set; } = new List<ProductOptionValueDto>();
    }

    public class ProductOptionValueDto
    {
        public Guid Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }
}
