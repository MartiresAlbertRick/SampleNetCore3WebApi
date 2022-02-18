using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class NonPoVendorValidator : BaseValidator<NonPoVendor>
    {
        public NonPoVendorValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.EntityCode).Length(0, 10).NotNull();
            RuleFor(entity => entity.EntityName).Length(0, 100);
            RuleFor(entity => entity.VendorBusinessNumber).Length(0, 50).NotNull();
            RuleFor(entity => entity.VendorName).Length(0, 200).NotNull();
            RuleFor(entity => entity.BranchCode).Length(0, 20);
            RuleFor(entity => entity.BranchName).Length(0, 100);
            RuleFor(entity => entity.DivisionCode).Length(0, 10);
            RuleFor(entity => entity.DivisionName).Length(0, 100);
            RuleFor(entity => entity.BusinessUnitCode).Length(0, 100);
            RuleFor(entity => entity.BusinessUnitName).Length(0, 100);
            RuleFor(entity => entity.SiteCode).Length(0, 10);
            RuleFor(entity => entity.SiteName).Length(0, 100);
            RuleFor(entity => entity.DefaultGLCode).Length(0, 50);
            RuleFor(entity => entity.AccountNumber).Length(0, 50);
            RuleFor(entity => entity.Custom01).Length(0, 50);
            RuleFor(entity => entity.Custom02).Length(0, 50);
            RuleFor(entity => entity.Custom03).Length(0, 50);
            RuleFor(entity => entity.Custom04).Length(0, 50);
            RuleFor(entity => entity.VendorBusinessNumberNormalized).Length(0, 50);
            RuleFor(entity => entity.AccountNumberNormalized).Length(0, 50);
            RuleFor(entity => entity.UpdatedByUser).Length(0, 200);
            RuleFor(entity => entity.RoutingCode).Length(0, 50);
            RuleFor(entity => entity.PoNumber).Length(0, 50);
        }
    }
}