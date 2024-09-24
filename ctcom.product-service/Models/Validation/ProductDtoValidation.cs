using FluentValidation;

namespace ctcom.ProductService.DTOs.Validation
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("Product title is required.")
                .MaximumLength(100).WithMessage("Product title cannot exceed 100 characters.");

            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.");

            RuleFor(p => p.IsPublished)
                .Must(value => value == true || value == false).WithMessage("Invalid publication status.");

            RuleForEach(p => p.Variants).SetValidator(new ProductVariantDtoValidator());
        }
    }

    public class ProductVariantDtoValidator : AbstractValidator<ProductVariantDto>
    {
        public ProductVariantDtoValidator()
        {
            RuleFor(v => v.Title)
                .NotEmpty().WithMessage("Variant title is required.")
                .MaximumLength(50).WithMessage("Variant title cannot exceed 50 characters.");

            RuleFor(v => v.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to zero.");

            RuleFor(v => v.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to zero.");
        }
    }

    public class ProductOptionDtoValidator : AbstractValidator<ProductOptionDto>
    {
        public ProductOptionDtoValidator()
        {
            RuleFor(o => o.Title)
                .NotEmpty().WithMessage("Option title is required.");
        }
    }
}
