using System;
using System.Collections.Generic;
using ctcom.ProductService.DTOs;
using ctcom.ProductService.Models;

namespace ctcom.ProductService.Events
{
    public class ProductCreatedEvent
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Handle { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public List<ProductVariantDto> Variants { get; set; } = new();
        public List<ProductOptionDto> Options { get; set; } = new();
        public List<ProductImageDto> Images { get; set; } = new();

        // Use Product as the input instead of CreateProductDto
        public ProductCreatedEvent(Product product)
        {
            ProductId = product.Id;
            Title = product.Title;
            Description = product.Description;
            Handle = product.Handle;
            IsPublished = product.IsPublished;
            CreatedAt = product.CreatedAt;
            PublishedAt = product.PublishedAt;

            // Map variants, options, and images
            Variants = new List<ProductVariantDto>();
            foreach (var variant in product.Variants)
            {
                Variants.Add(new ProductVariantDto
                {
                    Id = variant.Id,
                    Title = variant.Title,
                    Price = variant.Price,
                    StockQuantity = variant.StockQuantity
                });
            }

            Options = new List<ProductOptionDto>();
            foreach (var option in product.Options)
            {
                Options.Add(new ProductOptionDto
                {
                    Id = option.Id,
                    Title = option.Title
                });
            }

            Images = new List<ProductImageDto>();
            foreach (var image in product.Images)
            {
                Images.Add(new ProductImageDto
                {
                    Id = image.Id,
                    Url = image.Url,
                    AltText = image.AltText
                });
            }
        }
    }
}
