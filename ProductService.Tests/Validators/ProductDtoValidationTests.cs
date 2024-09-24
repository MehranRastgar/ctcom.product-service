using Xunit;
using FluentValidation.TestHelper;
using ctcom.ProductService.DTOs;
using ctcom.ProductService.DTOs.Validation;

namespace ctcom.ProductService.Tests
{
    public class ProductDtoValidationTests
    {
        private readonly ProductDtoValidator _validator;

        public ProductDtoValidationTests()
        {
            _validator = new ProductDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var product = new ProductDto { Title = "" };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_MaxLength()
        {
            var product = new ProductDto { Title = new string('A', 101) };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Negative_In_Variant()
        {
            var product = new ProductDto
            {
                Variants = new List<ProductVariantDto>
                {
                    new ProductVariantDto { Title = "Variant 1", Price = -1 }
                }
            };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor("Variants[0].Price");
        }
    }
}
