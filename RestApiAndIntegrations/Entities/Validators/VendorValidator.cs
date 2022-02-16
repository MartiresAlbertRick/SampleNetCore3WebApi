using FluentValidation;

namespace AD.CAAPS.Entities.Validators
{
    public class VendorValidator : BaseValidator<Vendor>
    {
        public VendorValidator()
        {
            RuleFor(entity => entity.ID).NotNull();
            RuleFor(entity => entity.VendorUID).Length(0, 100);
            RuleFor(entity => entity.EntityCode).Length(0, 10);
            RuleFor(entity => entity.VendorName).Length(0, 250).NotNull();
            RuleFor(entity => entity.VendorCode).Length(0, 50).NotNull();
            RuleFor(entity => entity.VendorBusinessNumber).Length(0, 20);
            RuleFor(entity => entity.VendorARContactName).Length(0, 100);
            RuleFor(entity => entity.VendorARContactEmailAddress).Length(0, 100);
            RuleFor(entity => entity.VendorAddressLine01).Length(0, 100);
            RuleFor(entity => entity.VendorAddressLine02).Length(0, 100);
            RuleFor(entity => entity.VendorAddressLine03).Length(0, 100);
            RuleFor(entity => entity.VendorAddressLine04).Length(0, 100);
            RuleFor(entity => entity.VendorSuburb).Length(0, 50);
            RuleFor(entity => entity.VendorState).Length(0, 50);
            RuleFor(entity => entity.VendorPostCode).Length(0, 15);
            RuleFor(entity => entity.VendorCity).Length(0, 50);
            RuleFor(entity => entity.VendorCountry).Length(0, 50);
            RuleFor(entity => entity.BankBsbNumber).Length(0, 50);
            RuleFor(entity => entity.BankAccountNumber).Length(0, 50);
            RuleFor(entity => entity.BPAYBillerCode).Length(0, 10);
            RuleFor(entity => entity.BPAYReferenceNumber).Length(0, 20);
            RuleFor(entity => entity.PaymentTypeCode).Length(0, 20);
            RuleFor(entity => entity.PaymentTermsCode).Length(0, 20);
            RuleFor(entity => entity.VendorCurrencyCode).Length(0, 3);
            RuleFor(entity => entity.PoRequiredYN).Must(value => value == null || value == false || value == true);
            RuleFor(entity => entity.Custom01).Length(0, 100);
            RuleFor(entity => entity.Custom02).Length(0, 100);
            RuleFor(entity => entity.Custom03).Length(0, 100);
            RuleFor(entity => entity.Custom04).Length(0, 100);
        }
    }
}