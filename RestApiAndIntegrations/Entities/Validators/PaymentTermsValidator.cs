using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class PaymentTermsValidator : BaseValidator<PaymentTerms>
    {
        public PaymentTermsValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.PaymentTermsCode).MaximumLength(20).NotNull();
            RuleFor(entity => entity.PaymentTermsDescription).MaximumLength(100);
            RuleFor(entity => entity.Custom01).MaximumLength(100);
            RuleFor(entity => entity.Custom02).MaximumLength(100);
            RuleFor(entity => entity.Custom03).MaximumLength(100);
            RuleFor(entity => entity.Custom04).MaximumLength(100);
        }
    }
}