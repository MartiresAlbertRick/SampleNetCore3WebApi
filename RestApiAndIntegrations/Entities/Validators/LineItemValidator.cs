using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class LineItemValidator : BaseValidator<LineItem>
    {
        public LineItemValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.LineHeaderUID).Length(0, 20);
            RuleFor(entity => entity.LinePoNumber).Length(0, 50);
            RuleFor(entity => entity.LineOriginalPoNumber).Length(0, 25);
            RuleFor(entity => entity.LineProductCode).Length(0, 50);
            RuleFor(entity => entity.LineOriginalProductCode).Length(0, 50);
            RuleFor(entity => entity.LineValidAdditionalCharge).Length(0, 100);
            RuleFor(entity => entity.LineUOM).Length(0, 50);
            RuleFor(entity => entity.LineDescription).Length(0, 512);
            RuleFor(entity => entity.PoIssuedBy).Length(0, 100);
            RuleFor(entity => entity.PoType).Length(0, 10);
            RuleFor(entity => entity.LinePoLineNumber).Length(0, 10);
            RuleFor(entity => entity.LineTaxCode).Length(0, 10);
        }
    }
}