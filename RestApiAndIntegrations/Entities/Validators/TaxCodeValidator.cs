using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class TaxCodeValidator : BaseValidator<TaxCodeDetails>
    {
        public TaxCodeValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.TaxCode).MaximumLength(50).NotNull();
            RuleFor(entity => entity.TaxDescription).MaximumLength(100).NotNull();
            RuleFor(entity => entity.Custom01).MaximumLength(100);
            RuleFor(entity => entity.Custom02).MaximumLength(100);
            RuleFor(entity => entity.Custom03).MaximumLength(100);
            RuleFor(entity => entity.Custom04).MaximumLength(100);
            RuleFor(entity => entity.ProcessingCurrency).MaximumLength(3);
            RuleFor(entity => entity.DefaultTaxCodeFlag).Must(value => value == null || value == false || value == true);
        }
    }
}