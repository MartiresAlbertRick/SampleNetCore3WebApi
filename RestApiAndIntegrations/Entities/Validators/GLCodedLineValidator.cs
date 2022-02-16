using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class GLCodedLineValidator : BaseValidator<GLCodeLine>
    {
        public GLCodedLineValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.LineAccountType).Length(0, 50);
            RuleFor(entity => entity.LineAccountCodeA).Length(0, 50);
            RuleFor(entity => entity.LineAccountCodeB).Length(0, 50);
            RuleFor(entity => entity.LineAccountCodeC).Length(0, 50);
            RuleFor(entity => entity.LineAccountCodeD).Length(0, 50);
            RuleFor(entity => entity.LineAccountCodeE).Length(0, 50);
            RuleFor(entity => entity.LineAccountCodeF).Length(0, 50);
            RuleFor(entity => entity.LineDescription).Length(0, 255);
            RuleFor(entity => entity.LineCustomFieldA).Length(0, 50);
            RuleFor(entity => entity.LineCustomFieldB).Length(0, 50);
            RuleFor(entity => entity.LineCustomFieldC).Length(0, 50);
            RuleFor(entity => entity.LineCustomFieldD).Length(0, 50);
            RuleFor(entity => entity.LineTaxCode).Length(0, 50);
            RuleFor(entity => entity.GLCode).Length(0, 50);
            RuleFor(entity => entity.GLCodeDesc).Length(0, 255);
            RuleFor(entity => entity.LineVACDesc).Length(0, 250);

            RuleFor(entity => entity.LineApprovedYN).Must(value => value == null || value == false || value == true);
        }
    }
}