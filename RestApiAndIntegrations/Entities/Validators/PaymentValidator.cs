using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class PaymentValidator : BaseValidator<Payment>
    {
        public PaymentValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.CaapsUniqueId).Length(0, 9);
            RuleFor(entity => entity.PaymentStatus).Length(0, 50);
            RuleFor(entity => entity.PaymentAmount).NotNull();
            RuleFor(entity => entity.PaymentAmount).Must(BeGreaterThanZero).WithMessage("Required payment amount should be greater than zero.");
            RuleFor(entity => entity.PaymentMethod).Length(0, 30);
            RuleFor(entity => entity.ClientTransactionId).Length(0, 50);
            RuleFor(entity => entity.PaymentBatchNumber).Length(0, 50);
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
        }
    }
}