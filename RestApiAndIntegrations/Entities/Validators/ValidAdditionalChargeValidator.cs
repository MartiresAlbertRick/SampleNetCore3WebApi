using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class ValidAdditionalChargeValidator : BaseValidator<ValidAdditionalCharges>
    {
        public ValidAdditionalChargeValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.EntityCode).Length(0, 10).NotNull();
            RuleFor(entity => entity.EntityName).Length(0, 100);
            RuleFor(entity => entity.ValidAdditionalCharge).Length(0, 50).NotNull();
            RuleFor(entity => entity.DefaultGLCode).Length(0, 50);
            RuleFor(entity => entity.ValidAdditionalCharge).Length(0, 50);
            RuleFor(entity => entity.DefaultGLCode).Length(0, 50);
            RuleFor(entity => entity.ValidAlternatives).Length(0, 2000);
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
            RuleFor(entity => entity.UpdatedByUser).Length(0, 200);
        }
    }
}