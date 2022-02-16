using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class GLCodeValidator : BaseValidator<GLCodeDetails>
    {
        public GLCodeValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.EntityCode).MaximumLength(10).NotNull();
            RuleFor(entity => entity.EntityName).MaximumLength(100);
            RuleFor(entity => entity.GLCode).MaximumLength(100).NotNull();
            RuleFor(entity => entity.GLCodeDescription).MaximumLength(100).NotNull();
            RuleFor(entity => entity.Custom01).MaximumLength(100);
            RuleFor(entity => entity.Custom02).MaximumLength(100);
            RuleFor(entity => entity.Custom03).MaximumLength(100);
            RuleFor(entity => entity.Custom04).MaximumLength(100);
        }
    }
}