using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class ImportConfirmationValidator : BaseValidator<ImportConfirmation>
    {
        public ImportConfirmationValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.CaapsUniqueId).Length(0, 9);
            RuleFor(entity => entity.ImportStatus).Length(0, 50);
            RuleFor(entity => entity.ClientTransactionId).Length(0, 50);
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
        }
    }
}