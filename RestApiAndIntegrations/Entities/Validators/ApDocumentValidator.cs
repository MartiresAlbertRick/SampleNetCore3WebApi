using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class ApDocumentValidator : BaseValidator<ApDocument>
    {
        public ApDocumentValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.DocHeaderUID).Length(0, 20).NotNull();
            RuleFor(entity => entity.CaapsRecordId).Length(0, 50);

            RuleFor(entity => entity.DocRefNumberA).Length(0, 50);
            RuleFor(entity => entity.ProcessType).Length(0, 50);
            RuleFor(entity => entity.EntityCode).Length(0, 10);
            RuleFor(entity => entity.EntityName).Length(0, 100);
            RuleFor(entity => entity.BranchCode).Length(0, 10);
            RuleFor(entity => entity.BranchName).Length(0, 100);
            RuleFor(entity => entity.SiteCode).Length(0, 10);
            RuleFor(entity => entity.SiteName).Length(0, 100);
            RuleFor(entity => entity.BusinessUnitCode).Length(0, 10);
            RuleFor(entity => entity.BusinessUnitName).Length(0, 100);
            RuleFor(entity => entity.DivisionCode).Length(0, 10);
            RuleFor(entity => entity.DivisionName).Length(0, 100);

            RuleFor(entity => entity.VendorCode).Length(0, 20);
            RuleFor(entity => entity.VendorName).Length(0, 100);
            RuleFor(entity => entity.VendorBusinessNumber).Length(0, 20);
            RuleFor(entity => entity.VendorBankBsb).Length(0, 50);
            RuleFor(entity => entity.VendorBankAccountNumber).Length(0, 50);
            RuleFor(entity => entity.BPAYBillerCode).Length(0, 10);
            RuleFor(entity => entity.BPAYReferenceNumber).Length(0, 20);
            RuleFor(entity => entity.PaymentTypeCode).Length(0, 20);

            RuleFor(entity => entity.DocDescription).Length(0, 200);
            RuleFor(entity => entity.ProcessStatusCurrent).Length(0, 50);
            RuleFor(entity => entity.Custom01).Length(0, 50);
            RuleFor(entity => entity.Custom02).Length(0, 50);
            RuleFor(entity => entity.Custom03).Length(0, 50);
            RuleFor(entity => entity.Custom04).Length(0, 50);

            RuleFor(entity => entity.RoleIDs).Length(0, 250);
            RuleFor(entity => entity.UniqueIdentifier).Length(0, 50);
            RuleFor(entity => entity.FileName).Length(0, 250);
            RuleFor(entity => entity.FileURL).Length(0, 500);
            RuleFor(entity => entity.FromEmailAddress).Length(0, 500);
            RuleFor(entity => entity.DocType).Length(0, 50);

            RuleFor(entity => entity.AccountNumber).Length(0, 50);
            RuleFor(entity => entity.DocAmountCurrency).Length(0, 10);
            RuleFor(entity => entity.StatementInvoiceNumber).Length(0, 4000);
            RuleFor(entity => entity.RoutingCode).Length(0, 50);
            RuleFor(entity => entity.DocReceivedSource).Length(0, 100);
            RuleFor(entity => entity.DocReceivedType).Length(0, 10);
            RuleFor(entity => entity.DocMultiplePoInHeaderYN).Must(value => value == null || value == false || value == true);

            var glValidator = new GLCodedLineValidator();
            RuleForEach(entity => entity.AccountCodingLines).SetValidator(glValidator);
            var lineItemValidator = new LineItemValidator();
            RuleForEach(entity => entity.LineItems).SetValidator(lineItemValidator);
        }
    }
}