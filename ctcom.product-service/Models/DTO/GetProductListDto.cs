namespace ctcom.ProductService.DTOs
{
    public class GetProductListDto
    {
        public Guid Id { get; set; }    // Product ID
        public string Title { get; set; } = string.Empty;    // Main product title
        public string Description { get; set; } = string.Empty;    // Product description
        public string Handle { get; set; } = string.Empty;    // URL handle or slug
        public bool IsPublished { get; set; } = false;    // Whether the product is live
        public DateTime? PublishedAt { get; set; }    // Date of publishing (optional)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    // Creation timestamp
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;    // Last update timestamp

        // Optional: You can include additional fields like number of variants, options, images if needed

        public List<GetProductVariantDto> Variants { get; set; } = new List<GetProductVariantDto>();
        public List<GetProductOptionDto> Options { get; set; } = new List<GetProductOptionDto>();
        public List<GetProductImageDto> Images { get; set; } = new List<GetProductImageDto>();
    }
}
