using System;
using System.Collections.Generic;

namespace ctcom.ProductService.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;            // Main product title
        public string Description { get; set; } = string.Empty;      // Product description
        public string Handle { get; set; } = string.Empty;           // URL handle or slug
        public bool IsPublished { get; set; } = false;               // Whether the product is live
        public DateTime? PublishedAt { get; set; }                   // Date of publishing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   // Creation timestamp
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;   // Last update timestamp

        // Relationships
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>(); // Associated variants
        public ICollection<ProductOption> Options { get; set; } = new List<ProductOption>();   // Product options like size, color
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();      // Product images
    }

    // Represents product variants (e.g., different sizes or colors)
    public class ProductVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;            // Variant title
        public decimal Price { get; set; }                           // Price for this variant
        public int StockQuantity { get; set; }                       // Stock available for this variant
        public Guid ProductId { get; set; }                          // Reference to the parent product

        // Relationships
        public Product Product { get; set; }                         // The parent product
        public ICollection<ProductOptionValue> OptionValues { get; set; } = new List<ProductOptionValue>(); // Variant's options (e.g., Red, Large)
    }

    // Represents the values for a product option (e.g., Red, Large)
    public class ProductOptionValue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; } = string.Empty;            // Option value (e.g., Red, Large)
        public Guid ProductOptionId { get; set; }                    // Reference to the parent option

        // Relationships
        public ProductOption ProductOption { get; set; }             // The parent option (e.g., Size, Color)
    }
    public class ProductOption
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;

        public Guid ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; } // Navigation property
    }

    // Represents product images
    public class ProductImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Url { get; set; }
        public string AltText { get; set; } = string.Empty;

        public Guid ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; } // Navigation property
    }



}
