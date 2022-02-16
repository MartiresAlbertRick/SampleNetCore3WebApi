using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class RoutingCodeValidator : BaseValidator<RoutingCodes>
    {
        public RoutingCodeValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.EntityCode).Length(0, 10).NotNull();
            RuleFor(entity => entity.EntityName).Length(0, 100);
            RuleFor(entity => entity.RoutingCode).Length(0, 50);
            RuleFor(entity => entity.FirstRecipientType).Length(0, 50);
            RuleFor(entity => entity.FirstRecipient).Length(0, 50);
            RuleFor(entity => entity.FirstRecipientFullName).Length(0, 100);
            RuleFor(entity => entity.BranchCode).Length(0, 0);
            RuleFor(entity => entity.BranchName).Length(0, 100);
            RuleFor(entity => entity.DivisionCode).Length(0, 10);
            RuleFor(entity => entity.DivisionName).Length(0, 100);
            RuleFor(entity => entity.BusinessUnitCode).Length(0, 100);
            RuleFor(entity => entity.BusinessUnitName).Length(0, 100);
            RuleFor(entity => entity.SiteCode).Length(0, 10);
            RuleFor(entity => entity.SiteName).Length(0, 100);
            RuleFor(entity => entity.DefaultGLCode).Length(0, 50);
            RuleFor(entity => entity.Custom01).Length(0, 2000);
            RuleFor(entity => entity.Custom02).Length(0, 50);
            RuleFor(entity => entity.Custom03).Length(0, 50);
            RuleFor(entity => entity.Custom04).Length(0, 50);
            RuleFor(entity => entity.UpdatedByUser).Length(0, 200);
        }
    }
}