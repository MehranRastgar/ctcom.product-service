using AutoMapper;
using ctcom.ProductService.Models;
using ctcom.ProductService.DTOs;

namespace ctcom.ProductService.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();
            CreateMap<ProductOption, ProductOptionDto>().ReverseMap();
            CreateMap<ProductOptionValue, ProductOptionValueDto>().ReverseMap();
            CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        }
    }
}
