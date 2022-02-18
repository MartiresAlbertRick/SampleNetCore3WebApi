using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class CurrencyValidator : BaseValidator<Currency>
    {
        public CurrencyValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.CurrencyCode).Length(0, 10);
            RuleFor(entity => entity.CurrencyName).Length(0, 200);
        }
    }
}