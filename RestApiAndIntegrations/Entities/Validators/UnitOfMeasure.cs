using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class UnitOfMeasureValidator : BaseValidator<UnitOfMeasure>
    {
        public UnitOfMeasureValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.UOMCode).Length(0, 50).NotNull();
            RuleFor(entity => entity.UOMDescription).Length(0, 100);
            RuleFor(entity => entity.UOMAlternatives).Length(0, 2000);
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
            RuleFor(entity => entity.UpdatedByUser).Length(0, 200);
        }
    }
}