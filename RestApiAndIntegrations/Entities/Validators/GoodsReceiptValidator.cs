using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class GoodsReceiptValidator : BaseValidator<GoodsReceipt>
    {
        public GoodsReceiptValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.EntityCode).Length(0, 10);
            RuleFor(entity => entity.ReceivedDate).NotNull();
            RuleFor(entity => entity.ReceivedBy).Length(0, 100).NotNull();
            RuleFor(entity => entity.PoNumber).Length(0, 25).NotNull();
            RuleFor(entity => entity.PoLineNumber).NotNull();
            RuleFor(entity => entity.ReceiptedQty).NotNull();
            RuleFor(entity => entity.ReceiptedQty).Must(BeGreaterThanZero).WithMessage("Required receipted quantity should be greater than zero.");
            RuleFor(entity => entity.GoodsReceivedNumber).Length(0, 10).NotNull();
            RuleFor(entity => entity.ReceiptedValueTaxEx).NotNull();
            RuleFor(entity => entity.ReceiptedValueTaxEx).Must(BeGreaterThanZero).WithMessage("Required receipted value tax ex should be greater than zero.");
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
        }
    }
}