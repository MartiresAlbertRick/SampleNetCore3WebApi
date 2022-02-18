using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class ClosedPurchaseOrderValidator : BaseValidator<ClosedPurchaseOrder>
    {
        public ClosedPurchaseOrderValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.PoNumber).Length(0, 25).NotNull();
            RuleFor(entity => entity.VendorCode).Length(0, 50);
            RuleFor(entity => entity.PoRaisedDate).Length(0, 10);
            RuleFor(entity => entity.EntityCode).Length(0, 10);
            RuleFor(entity => entity.PoType).Length(0, 20);
            RuleFor(entity => entity.PoDate).Length(0, 10);
            RuleFor(entity => entity.PoRaisedByUsername).Length(0, 200);
            RuleFor(entity => entity.JobNumber).Length(0, 8);
            RuleFor(entity => entity.CurrencyCode).Length(0, 20);
            RuleFor(entity => entity.PoStatusCode).Length(0, 10);
            RuleFor(entity => entity.PayToVendorCode).Length(0, 50);
            RuleFor(entity => entity.VendorCode).Length(0, 50);
            RuleFor(entity => entity.SiteCode).Length(0, 10);
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
        }
    }
}