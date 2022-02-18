using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class EntityValidator : BaseValidator<Entity>
    {
        public EntityValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.EntityCode).Length(0, 10);
            RuleFor(entity => entity.EntityCodeEquivalent).Length(0, 10);
            RuleFor(entity => entity.EntityName).Length(0, 100);
            RuleFor(entity => entity.EntityNameEquivalent).Length(0, 4000);
            RuleFor(entity => entity.BranchCode).Length(0, 10);
            RuleFor(entity => entity.BranchName).Length(0, 100);
            RuleFor(entity => entity.DivisionCode).Length(0, 10);
            RuleFor(entity => entity.DivisionName).Length(0, 100);
            RuleFor(entity => entity.BusinessUnitCode).Length(0, 10);
            RuleFor(entity => entity.BusinessUnitName).Length(0, 100);
            RuleFor(entity => entity.SiteCode).Length(0, 20);
            RuleFor(entity => entity.SiteName).Length(0, 100);
            RuleFor(entity => entity.DefaultContactUserName).Length(0, 50);
            RuleFor(entity => entity.EmailSignOff).Length(0, 4000);
            RuleFor(entity => entity.ReplyToAddress).Length(0, 100);
            RuleFor(entity => entity.BccAddress).Length(0, 100);
            RuleFor(entity => entity.Custom01).Length(0, 50);
            RuleFor(entity => entity.Custom02).Length(0, 50);
            RuleFor(entity => entity.Custom03).Length(0, 50);
            RuleFor(entity => entity.Custom04).Length(0, 50);
            RuleFor(entity => entity.ClientCaapsProcessingAddress).Length(0, 100);
            RuleFor(entity => entity.ClientAPQueriesAddress).Length(0, 100);
            RuleFor(entity => entity.ProcessingCurrency).Length(0, 3);
            RuleFor(entity => entity.UpdatedByUser).Length(0, 200);
            RuleFor(entity => entity.ReferenceAddress).Length(0, 4000);
            RuleFor(entity => entity.EntityBusinessNumber).Length(0, 20);
        }
    }
}