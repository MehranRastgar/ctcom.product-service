using System;
using System.Collections.Generic;
using ctcom.ProductService.DTOs;

namespace ctcom.ProductService.Events
{
    public class ProductUpdatedEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        // Include the associated DTOs for variants, options, and images
        public ICollection<ProductVariantDto> Variants { get; set; }
        public ICollection<ProductOptionDto> Options { get; set; }
        public ICollection<ProductImageDto> Images { get; set; }

        public ProductUpdatedEvent(ProductDto product)
        {
            Id = product.Id;
            Title = product.Title;
            Description = product.Description;
            IsPublished = product.IsPublished;
            UpdatedAt = product.UpdatedAt;
            PublishedAt = product.PublishedAt;
            Variants = product.Variants;
            Options = product.Options;
            Images = product.Images;
        }
    }
}
