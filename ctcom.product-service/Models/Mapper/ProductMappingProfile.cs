using AutoMapper;
using ctcom.ProductService.Models;
using ctcom.ProductService.DTOs;

namespace ctcom.ProductService.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Mapping for Create operation
            CreateMap<Product, CreateProductDto>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ReverseMap();

            // Mapping for Update operation
            CreateMap<Product, UpdateProductDto>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ReverseMap();

            // Mapping for Get operation (single product)
            CreateMap<Product, GetProductDto>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ReverseMap();

            // Mapping for List/Get multiple products (pagination)
            CreateMap<Product, GetProductListDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Handle, opt => opt.MapFrom(src => src.Handle))
                .ReverseMap();

            // Mapping for Delete operation (though DeleteProductDto may not need ReverseMap)
            CreateMap<Product, DeleteProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            // Mapping for Product Variants
            CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();
            CreateMap<ProductVariant, CreateProductVariantDto>().ReverseMap();
            CreateMap<ProductVariant, UpdateProductVariantDto>().ReverseMap();
            CreateMap<ProductVariant, GetProductVariantDto>().ReverseMap();

            // Mapping for Product Option and Option Values
            CreateMap<ProductOption, ProductOptionDto>().ReverseMap();
            CreateMap<ProductOption, CreateProductOptionDto>().ReverseMap();
            CreateMap<ProductOption, UpdateProductOptionDto>().ReverseMap();
            CreateMap<ProductOptionValue, ProductOptionValueDto>().ReverseMap();
            CreateMap<ProductOptionValue, CreateProductOptionValueDto>().ReverseMap();
            CreateMap<ProductOptionValue, UpdateProductOptionValueDto>().ReverseMap();

            // Mapping for Product Images
            CreateMap<ProductImage, ProductImageDto>().ReverseMap();
            CreateMap<ProductImage, CreateProductImageDto>().ReverseMap();
            CreateMap<ProductImage, UpdateProductImageDto>().ReverseMap();
        }
    }
}
