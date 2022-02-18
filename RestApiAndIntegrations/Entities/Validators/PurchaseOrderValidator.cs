using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class PurchaseOrderValidator : BaseValidator<PurchaseOrder>
    {
        public PurchaseOrderValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.EntityCode).NotNull().Length(0, 10);
            RuleFor(entity => entity.PoDate).NotNull();
            RuleFor(entity => entity.PoRaisedByUsername).Length(0, 100);
            RuleFor(entity => entity.PoRequisitionerUserName).Length(0, 100);
            RuleFor(entity => entity.PoNumber).Length(0, 25).NotNull();
            RuleFor(entity => entity.PoType).Length(0, 50);
            RuleFor(entity => entity.PoStatusCode).NotNull().Length(0, 50);
            RuleFor(entity => entity.CurrencyCode).Length(0, 20);
            RuleFor(entity => entity.LineNumber).NotNull();
            RuleFor(entity => entity.LineNumber).Must(BeGreaterThanZero).WithMessage("Required line number should be greater than zero.");
            RuleFor(entity => entity.LineProductCode).Length(0, 50);
            RuleFor(entity => entity.LineOpenQuantity).NotNull();
            RuleFor(entity => entity.LineOpenQuantity).Must(BeGreaterThanZero).WithMessage("Required line open quantity should be greater than zero.");
            RuleFor(entity => entity.LineUOM).Length(0, 50);
            RuleFor(entity => entity.LineUnitPrice).NotNull();
            RuleFor(entity => entity.LineUnitPrice).Must(BeGreaterThanZero).WithMessage("Required line unit price should be greater than zero.");
            RuleFor(entity => entity.LineTaxCode).Length(0, 10);
            RuleFor(entity => entity.LineGLCode).Length(0, 100);
            RuleFor(entity => entity.LineDescription).Length(0, 200);
            RuleFor(entity => entity.VendorCode).NotNull().Length(0, 20);
            RuleFor(entity => entity.BranchCode).Length(0, 10);
            RuleFor(entity => entity.DivisionCode).Length(0, 10);
            RuleFor(entity => entity.SiteCode).Length(0, 10);
            RuleFor(entity => entity.BusinessUnitCode).Length(0, 10);
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
        }
    }
}