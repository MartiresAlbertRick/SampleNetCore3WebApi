using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class ProductValidator : BaseValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.VendorBusinessNumber).MaximumLength(20).NotNull();
            RuleFor(entity => entity.VendorName).MaximumLength(250);
            RuleFor(entity => entity.VendorProductCode).MaximumLength(50).NotNull();
            RuleFor(entity => entity.VendorProductDescription).MaximumLength(250);
            RuleFor(entity => entity.VendorUOM).MaximumLength(50);
            RuleFor(entity => entity.PoProductCode).MaximumLength(50).NotNull();
            RuleFor(entity => entity.PoProductDescription).MaximumLength(250);
            RuleFor(entity => entity.PoUOM).MaximumLength(50);
            RuleFor(entity => entity.Custom01).MaximumLength(100);
            RuleFor(entity => entity.Custom02).MaximumLength(100);
            RuleFor(entity => entity.Custom03).MaximumLength(100);
            RuleFor(entity => entity.Custom04).MaximumLength(100);
        }
    }
}