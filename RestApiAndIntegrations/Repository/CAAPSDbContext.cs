using AD.CAAPS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace AD.CAAPS.Repository
{
    public class CAAPSDbContext : DbContext
    {
        public DBConfiguration DBConfiguration { get; set; }

        public CAAPSDbContext(DbContextOptions<CAAPSDbContext> options, DBConfiguration dBConfiguration) : base(options)
        {
            DBConfiguration = dBConfiguration;
        }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<ImportConfirmation> ImportConfirmations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ValidAdditionalCharges> ValidAdditionalCharges { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<RoutingCodes> RoutingCodes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<NonPoVendor> NonPoVendors { get; set; }
        public DbSet<ApDocument> ApDocuments { get; set; }
        public DbSet<GLCodeLine> AccountCodingLines { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<InvoiceLifeCycleEvent> InvoiceLifeCycleEvents { get; set; }
        public DbSet<SystemOption> SystemOptions { get; set; }
        public DbSet<FileLink> FileLinks { get; set; }
        public DbSet<FileNameRecord> FileNames { get; set; }
        public DbSet<S3DriveConfiguration> S3DriveConfigurations { get; set; }
        public DbSet<SundryPOGoodsAllocation> SundryPOGoodsAllocations { get; set; }
        public DbSet<GLCodeDetails> GLCodes { get; set; }
        public DbSet<TaxCodeDetails> TaxCodes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ClosedPurchaseOrder> ClosedPurchaseOrders { get; set; }
        public DbSet<PaymentTerms> PaymentTerms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureModelVendor(modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder)));
            ConfigureModelGoodsReceipt(modelBuilder);
            ConfigureModelPurchaseOrder(modelBuilder);
            ConfigureModelImportConfirmation(modelBuilder);
            ConfigureModelPayment(modelBuilder);
            ConfigureModelVAC(modelBuilder);
            ConfigureModelEntity(modelBuilder);
            ConfigureModelUOM(modelBuilder);
            ConfigureModelRoutingCodes(modelBuilder);
            ConfigureModelCurrency(modelBuilder);
            ConfigureModelNonPoVendor(modelBuilder);
            ConfigureModelApDocuments(modelBuilder);
            ConfigureModelGLCodeLine(modelBuilder);
            ConfigureModelLineItem(modelBuilder);
            ConfigureModelComments(modelBuilder);
            ConfigureModelInvoiceLifeCycleEvent(modelBuilder);
            ConfigureModelSystemOptions(modelBuilder);
            ConfigureModelFileLink(modelBuilder);
            ConfigureModelFileName(modelBuilder);
            ConfigureModelS3DriveConfiguration(modelBuilder);
            ConfigureModelSundryPOGoodsAllocations(modelBuilder);
            ConfigureModelGLCode(modelBuilder);
            ConfigureModelTaxCode(modelBuilder);
            ConfigureModelProduct(modelBuilder);
            ConfigureModelClosedPurchaseOrder(modelBuilder);
            ConfigureModelPaymentTerm(modelBuilder);
            ConfigureEntityRelationships(modelBuilder);
        }

        private void ConfigureModelVendor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vendor>().ToTable("BRE_CAAPS_VENDOR_DETAILS");

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.VendorUID).HasColumnName("VENDOR_UID").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorName).HasColumnName("VENDOR_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorCode).HasColumnName("VENDOR_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorBusinessNumber).HasColumnName("VENDOR_BUSINESS_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorARContactName).HasColumnName("VENDOR_AR_CONTACT_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorARContactEmailAddress).HasColumnName("VENDOR_AR_CONTACT_EMAIL_ADDRESS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorAddressLine01).HasColumnName("VENDOR_ADDRESS_LINE_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorAddressLine02).HasColumnName("VENDOR_ADDRESS_LINE_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorAddressLine03).HasColumnName("VENDOR_ADDRESS_LINE_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorAddressLine04).HasColumnName("VENDOR_ADDRESS_LINE_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorSuburb).HasColumnName("VENDOR_SUBURB").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorCity).HasColumnName("VENDOR_CITY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorState).HasColumnName("VENDOR_STATE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorPostCode).HasColumnName("VENDOR_POST_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorCountry).HasColumnName("VENDOR_COUNTRY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BankBsbNumber).HasColumnName("BANK_BSB_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BankAccountNumber).HasColumnName("BANK_ACCOUNT_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BPAYBillerCode).HasColumnName("BPAY_BILLER_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BPAYReferenceNumber).HasColumnName("BPAY_REFERENCE_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentTypeCode).HasColumnName("PAYMENT_TYPE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentTermsCode).HasColumnName("PAYMENT_TERMS_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorCurrencyCode).HasColumnName("VENDOR_CURRENCY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorBankAccountList).HasColumnName("VENDOR_BANK_ACCOUNT_LIST").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoRequiredYN).HasColumnName("PO_REQUIRED_YN").HasMaxLength(1).HasConversion(new BoolToStringConverter("N", "Y"));
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelGoodsReceipt(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GoodsReceipt>().ToTable("BRE_CAAPS_GOODS_RECEIVED");

            modelBuilder.Entity<GoodsReceipt>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ReceivedDate).HasColumnName("RECEIVED_DATE").HasConversion(CustomValueConverters.NullableDateTimeConverter(DBConfiguration.DateFormat));
                entity.Property(e => e.ReceivedBy).HasColumnName("RECEIVED_BY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoNumber).HasColumnName("PO_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoLineNumber).HasColumnName("PO_LINE_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ReceiptedQty).HasColumnName("RECEIPTED_QTY").HasConversion(CustomValueConverters.NullableDecimalConverter());
                entity.Property(e => e.GoodsReceivedNumber).HasColumnName("GOODS_RECEIVED_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ReceiptedValueTaxEx).HasColumnName("RECEIPTED_VALUE").HasConversion(CustomValueConverters.NullableDecimalConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelPurchaseOrder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrder>().ToTable("BRE_CAAPS_PURCHASE_ORDERS");

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoDate).HasColumnName("PO_DATE").HasConversion(CustomValueConverters.NullableDateTimeConverter(DBConfiguration.DateFormat));
                entity.Property(e => e.PoRaisedByUsername).HasColumnName("PO_RAISED_BY_USERNAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoRequisitionerUserName).HasColumnName("PO_REQUISITIONER_USERNAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoNumber).HasColumnName("PO_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoOriginalAmountTaxEx).HasColumnName("PO_ORIGINAL_AMOUNT_TAX_EX");
                entity.Property(e => e.PoOpenAmountTaxEx).HasColumnName("PO_OPEN_AMOUNT_TAX_EX");
                entity.Property(e => e.PoType).HasColumnName("PO_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoStatusCode).HasColumnName("PO_STATUS_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.CurrencyCode).HasColumnName("CURRENCY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ExpectedDate).HasColumnName("EXPECTED_DATE").HasConversion(CustomValueConverters.NullableDateTimeConverter(DBConfiguration.DateFormat));
                entity.Property(e => e.LineExpectedDate).HasColumnName("LINE_EXPECTED_DATE").HasConversion(CustomValueConverters.NullableDateTimeConverter(DBConfiguration.DateFormat));
                entity.Property(e => e.LineNumber).HasColumnName("LINE_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineProductCode).HasColumnName("LINE_PRODUCT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineOriginalQuantity).HasColumnName("LINE_ORIGINAL_QUANTITY");
                entity.Property(e => e.LineOpenQuantity).HasColumnName("LINE_OPEN_QUANTITY").HasConversion(CustomValueConverters.NullableDecimalConverter());
                entity.Property(e => e.LineOriginalAmountTaxEx).HasColumnName("LINE_ORIGINAL_AMOUNT_TAX_EX");
                entity.Property(e => e.LineOpenAmountTaxEx).HasColumnName("LINE_OPEN_AMOUNT_TAX_EX");
                entity.Property(e => e.LineUOM).HasColumnName("LINE_UOM").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineUnitPrice).HasColumnName("LINE_UNIT_PRICE").HasConversion(CustomValueConverters.NullableDecimalConverter());
                entity.Property(e => e.LineTaxCode).HasColumnName("LINE_TAX_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineGLCode).HasColumnName("LINE_GL_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineDescription).HasColumnName("LINE_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorCode).HasColumnName("VENDOR_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionCode).HasColumnName("DIVISION_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteCode).HasColumnName("SITE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitCode).HasColumnName("BUSINESS_UNIT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Ignore(e => e.GoodsReceipts);
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelImportConfirmation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportConfirmation>().ToTable("BRE_CUSTOM_IMPORT_CONFIRMATIONS");

            modelBuilder.Entity<ImportConfirmation>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.CaapsUniqueId).HasColumnName("CAAPS_UNIQUE_ID").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ImportStatus).HasColumnName("IMPORT_STATUS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ImportMessage).HasColumnName("IMPORT_MESSAGE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ClientTransactionId).HasColumnName("CLIENT_TRANSACTION_ID").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelPayment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>().ToTable("BRE_CAAPS_PAYMENTS");

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.CaapsUniqueId).HasColumnName("CAAPS_UNIQUE_ID").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentStatus).HasColumnName("PAYMENT_STATUS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentDate).HasColumnName("PAYMENT_DATE").HasConversion(CustomValueConverters.NullableDateTimeConverter(DBConfiguration.DateFormat));
                entity.Property(e => e.PaymentAmount).HasColumnName("PAYMENT_AMOUNT").HasConversion(CustomValueConverters.NullableDecimalConverter());
                entity.Property(e => e.PaymentMethod).HasColumnName("PAYMENT_METHOD").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ClientTransactionId).HasColumnName("CLIENT_TRANSACTION_ID").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentVoucherNumber).HasColumnName("PAYMENT_VOUCHER_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentBatchNumber).HasColumnName("PAYMENT_BATCH_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelVAC(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValidAdditionalCharges>().ToTable("BRE_CAAPS_VALID_ADDITIONAL_CHARGES");

            modelBuilder.Entity<ValidAdditionalCharges>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityName).HasColumnName("ENTITY_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ValidAdditionalCharge).HasColumnName("VALID_ADDITIONAL_CHARGE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DefaultGLCode).HasColumnName("DEFAULT_GL_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ValidAlternatives).HasColumnName("VALID_ALTERNATIVES").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UpdatedByUser).HasColumnName("UPDATED_BY_USER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>().ToTable("BRE_CAAPS_ENTITY_DETAILS");

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityCodeEquivalent).HasColumnName("ENTITY_CODE_EQUIVALENT").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityName).HasColumnName("ENTITY_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityNameEquivalent).HasColumnName("ENTITY_NAME_EQUIVALENT").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchName).HasColumnName("BRANCH_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionCode).HasColumnName("DIVISION_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionName).HasColumnName("DIVISION_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitCode).HasColumnName("BUSINESS_UNIT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitName).HasColumnName("BUSINESS_UNIT_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteCode).HasColumnName("SITE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteName).HasColumnName("SITE_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DefaultContactUserName).HasColumnName("DEFAULT_CONTACT_USER_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EmailSignOff).HasColumnName("EMAIL_SIGN_OFF").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ReplyToAddress).HasColumnName("REPLY_TO_ADDRESS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BccAddress).HasColumnName("BCC_ADDRESS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ClientCaapsProcessingAddress).HasColumnName("CLIENT_CAAPS_PROCESSING_ADDRESS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ClientAPQueriesAddress).HasColumnName("CLIENT_AP_QUERIES_ADDRESS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ProcessingCurrency).HasColumnName("PROCESSING_CURRENCY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UpdatedByUser).HasColumnName("UPDATED_BY_USER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityBusinessNumber).HasColumnName("ENTITY_BUSINESS_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ReferenceAddress).HasColumnName("REFERENCE_ADDRESS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelUOM(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitOfMeasure>().ToTable("BRE_CAAPS_UNIT_OF_MEASURE");

            modelBuilder.Entity<UnitOfMeasure>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.UOMCode).HasColumnName("UOM_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UOMDescription).HasColumnName("UOM_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UOMAlternatives).HasColumnName("UOM_ALTERNATIVES").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UpdatedByUser).HasColumnName("UPDATED_BY_USER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelRoutingCodes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoutingCodes>().ToTable("BRE_CAAPS_ROUTING_CODES");

            modelBuilder.Entity<RoutingCodes>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityName).HasColumnName("ENTITY_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.RoutingCode).HasColumnName("ROUTING_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FirstRecipientType).HasColumnName("FIRST_RECIPIENT_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FirstRecipient).HasColumnName("FIRST_RECIPIENT").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FirstRecipientFullName).HasColumnName("FIRST_RECIPIENT_FULL_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchName).HasColumnName("BRANCH_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionCode).HasColumnName("DIVISION_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionName).HasColumnName("DIVISION_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitCode).HasColumnName("BUSINESS_UNIT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitName).HasColumnName("BUSINESS_UNIT_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteCode).HasColumnName("SITE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteName).HasColumnName("SITE_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DefaultGLCode).HasColumnName("DEFAULT_GL_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UpdatedByUser).HasColumnName("UPDATED_BY_USER").HasConversion(CustomValueConverters.NullableStringConverter());
            });
        }

        private void ConfigureModelCurrency(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>().ToTable("BRE_CAAPS_CURRENCY_DETAILS");

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.CurrencyCode).HasColumnName("CURRENCY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.CountryName).HasColumnName("COUNTRY_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.CurrencyName).HasColumnName("CURRENCY_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
            });
        }

        private void ConfigureModelNonPoVendor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NonPoVendor>().ToTable("BRE_CAAPS_NON_PO_VENDORS");

            modelBuilder.Entity<NonPoVendor>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityName).HasColumnName("ENTITY_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorBusinessNumber).HasColumnName("VENDOR_BUSINESS_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorName).HasColumnName("VENDOR_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FirstRecipientType).HasColumnName("FIRST_RECIPIENT_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FirstRecipient).HasColumnName("FIRST_RECIPIENT").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FirstRecipientFullName).HasColumnName("FIRST_RECIPIENT_FULL_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchName).HasColumnName("BRANCH_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionCode).HasColumnName("DIVISION_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionName).HasColumnName("DIVISION_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitCode).HasColumnName("BUSINESS_UNIT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitName).HasColumnName("BUSINESS_UNIT_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteCode).HasColumnName("SITE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteName).HasColumnName("SITE_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DefaultGLCode).HasColumnName("DEFAULT_GL_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.AccountNumber).HasColumnName("ACCOUNT_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorBusinessNumberNormalized).HasColumnName("VENDOR_BUSINESS_NUMBER_NORMALIZED").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.AccountNumberNormalized).HasColumnName("ACCOUNT_NUMBER_NORMALIZED").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UpdatedByUser).HasColumnName("UPDATED_BY_USER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.RoutingCode).HasColumnName("ROUTING_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoNumber).HasColumnName("PO_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelApDocuments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApDocument>().ToTable("DRAWINGS");
            modelBuilder.Entity<ApDocument>().Ignore(t => t.LineItems);
            modelBuilder.Entity<ApDocument>().Ignore(t => t.AccountCodingLines);
            modelBuilder.Entity<ApDocument>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.ImportBatchID).HasColumnName("IMPORTBATCHID");
                entity.Property(e => e.RoleIDs).HasColumnName("ROLEIDS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.CaapsRecordId).HasColumnName("DV_RECORD_ID").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.RecordCreatedDate).HasColumnName("DV_RECORD_CREATED_DATE");
                entity.Property(e => e.DocReceivedSource).HasColumnName("AD_DOC_RECEIVED_SOURCE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocReceivedType).HasColumnName("AD_DOC_RECEIVED_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UniqueIdentifier).HasColumnName("AD_UNIQUE_IDENTIFIER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocDateReceived).HasColumnName("AD_DOC_DATE_RECEIVED");
                entity.Property(e => e.DocHeaderUID).HasColumnName("DOC_HEADER_UID").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocType).HasColumnName("DOC_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocDateIssued).HasColumnName("DOC_DATE_ISSUED");
                entity.Property(e => e.VendorCode).HasColumnName("VENDOR_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocRefNumberA).HasColumnName("DOC_REF_NUMBER_A").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocRefNumberB).HasColumnName("DOC_REF_NUMBER_B").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ProcessType).HasColumnName("PROCESS_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocAmountTax).HasColumnName("DOC_AMOUNT_TAX");
                entity.Property(e => e.DocAmountTaxEx).HasColumnName("DOC_AMOUNT_TAX_EX");
                entity.Property(e => e.DocAmountTotal).HasColumnName("DOC_AMOUNT_TOTAL");
                entity.Property(e => e.DocAmountCurrency).HasColumnName("DOC_AMOUNT_CURRENCY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityName).HasColumnName("ENTITY_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BranchName).HasColumnName("BRANCH_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitCode).HasColumnName("BUSINESS_UNIT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BusinessUnitName).HasColumnName("BUSINESS_UNIT_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteCode).HasColumnName("SITE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteName).HasColumnName("SITE_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionCode).HasColumnName("DIVISION_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DivisionName).HasColumnName("DIVISION_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ExportDate).HasColumnName("EXPORT_DATE");
                entity.Property(e => e.ProcessStatusCurrent).HasColumnName("PROCESS_STATUS_CURRENT").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoNumber).HasColumnName("PO_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocDescription).HasColumnName("DOC_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FileName).HasColumnName("DV_FILENAME_ORIGINAL").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FileURL).HasColumnName("FILE_URL").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.FromEmailAddress).HasColumnName("FROM_EMAIL_ADDRESS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorBusinessNumber).HasColumnName("VENDOR_BUSINESS_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorName).HasColumnName("VENDOR_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.AccountNumber).HasColumnName("ACCOUNT_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.StatementInvoiceNumber).HasColumnName("STATEMENT_INVOICE_NUMBERS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorBankBsb).HasColumnName("VENDOR_BANK_BSB").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorBankAccountNumber).HasColumnName("VENDOR_BANK_ACCOUNT_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BPAYBillerCode).HasColumnName("BPAY_BILLER_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.BPAYReferenceNumber).HasColumnName("BPAY_REFERENCE_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentTypeCode).HasColumnName("PAYMENT_TYPE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.RoutingCode).HasColumnName("ROUTING_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PaymentTermsCode).HasColumnName("VENDOR_PAYMENT_TERMS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocLineItemCount).HasColumnName("DOC_LINE_ITEM_COUNT");
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DocMultiplePoInHeaderYN).HasColumnName("DOC_MULTIPLE_PO_IN_HEADER_YN").HasMaxLength(1).HasConversion(new BoolToStringConverter("N", "Y"));
                entity.Property(e => e.DocDateDue).HasColumnName("DOC_DATE_DUE");
                entity.Property(e => e.DocPaymentType).HasColumnName("DOC_PAYMENT_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
            });
        }

        private void ConfigureModelGLCodeLine(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GLCodeLine>().ToTable("CAAPS_ACCOUNT_CODING_LINES");

            modelBuilder.Entity<GLCodeLine>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.RecordId).HasColumnName("RECORDID");
                entity.Property(e => e.LineNumber).HasColumnName("LINE_NUMBER");
                entity.Property(e => e.LineAccountType).HasColumnName("LINE_ACCOUNT_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineAccountCodeA).HasColumnName("LINE_ACCOUNT_CODE_A").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineAccountCodeB).HasColumnName("LINE_ACCOUNT_CODE_B").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineAccountCodeC).HasColumnName("LINE_ACCOUNT_CODE_C").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineAccountCodeD).HasColumnName("LINE_ACCOUNT_CODE_D").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineAccountCodeE).HasColumnName("LINE_ACCOUNT_CODE_E").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineAccountCodeF).HasColumnName("LINE_ACCOUNT_CODE_F").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineAmountTax).HasColumnName("LINE_AMOUNT_TAX");
                entity.Property(e => e.LineAmountTaxEx).HasColumnName("LINE_AMOUNT_TAX_EX");
                entity.Property(e => e.LineAmountTotal).HasColumnName("LINE_AMOUNT_TOTAL");
                entity.Property(e => e.LineApprovedYN).HasColumnName("LINE_APPROVED_YN").HasConversion(new BoolToStringConverter("N", "Y"));
                entity.Property(e => e.LineCalcPercent).HasColumnName("LINE_CALC_PERCENT");
                entity.Property(e => e.LineDescription).HasColumnName("LINE_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineCustomFieldA).HasColumnName("LINE_CUSTOM_FIELD_A").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineCustomFieldB).HasColumnName("LINE_CUSTOM_FIELD_B").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineCustomFieldC).HasColumnName("LINE_CUSTOM_FIELD_C").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineCustomFieldD).HasColumnName("LINE_CUSTOM_FIELD_D").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineTaxCode).HasColumnName("LINE_TAX_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.GLCode).HasColumnName("GL_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.GLCodeDesc).HasColumnName("GL_CODE_DESC").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineVACLineNumber).HasColumnName("LINE_VAC_LINE_NUMBER");
                entity.Property(e => e.LineVACDesc).HasColumnName("LINE_VAC_DESC").HasConversion(CustomValueConverters.NullableStringConverter());
            });
        }

        private void ConfigureModelLineItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LineItem>().ToTable("CAAPS_LINE_ITEMS");

            modelBuilder.Entity<LineItem>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.RecordId).HasColumnName("RECORDID");
                entity.Property(e => e.LineHeaderUID).HasColumnName("LINE_HEADER_UID");
                entity.Property(e => e.LineNumber).HasColumnName("LINE_NUMBER");
                entity.Property(e => e.LinePoLineNumber).HasColumnName("LINE_PO_LINE_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LinePoNumber).HasColumnName("LINE_PO_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineOriginalPoNumber).HasColumnName("LINE_ORIGINAL_PO_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineProductCode).HasColumnName("LINE_PRODUCT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineOriginalProductCode).HasColumnName("LINE_ORIGINAL_PRODUCT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineValidAdditionalCharge).HasColumnName("LINE_VALID_ADDITIONAL_CHARGE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineQuantity).HasColumnName("LINE_QUANTITY");
                entity.Property(e => e.LineUOM).HasColumnName("LINE_UOM").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineUnitAmountTaxEx).HasColumnName("LINE_UNIT_AMOUNT_TAX_EX");
                entity.Property(e => e.LineAmountTax).HasColumnName("LINE_AMOUNT_TAX");
                entity.Property(e => e.LineAmountTaxEx).HasColumnName("LINE_AMOUNT_TAX_EX");
                entity.Property(e => e.LineAmountTotal).HasColumnName("LINE_AMOUNT_TOTAL");
                entity.Property(e => e.LineDescription).HasColumnName("LINE_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoIssuedBy).HasColumnName("PO_ISSUED_BY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoType).HasColumnName("PO_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LineTaxCode).HasColumnName("LINE_TAX_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.OriginalUnitAmount).HasColumnName("ORIGINAL_UNIT_AMOUNT");
                entity.Property(e => e.LineOriginalAmountTotal).HasColumnName("LINE_ORIGINAL_AMOUNT_TOTAL");
            });
        }

        private void ConfigureModelComments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>().ToTable("CAAPS_USER_ACTIONS");

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.ManagedTableID).HasColumnName("MANAGEDTABLEID");
                entity.Property(e => e.RecordID).HasColumnName("RECORDID");
                entity.Property(e => e.UserName).HasColumnName("USERNAME");
                entity.Property(e => e.ActionName).HasColumnName("ACTION_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ActionComments).HasColumnName("ACTION_COMMENTS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.RecordStatus).HasColumnName("RECORD_STATUS").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ActionDetail).HasColumnName("ACTION_DETAIL").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ActionDetailA).HasColumnName("ACTION_DETAIL_A").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ActionDetailB).HasColumnName("ACTION_DETAIL_B").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ActionDetailC).HasColumnName("ACTION_DETAIL_C").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ActionStartDate).HasColumnName("ACTION_START_DATE");
                entity.Property(e => e.ActionEndDate).HasColumnName("ACTION_END_DATE");
                entity.Property(e => e.ExecutionTime).HasColumnName("EXECUTION_TIME");
                entity.Property(e => e.ColorIndex).HasColumnName("COLOR_INDEX");
            });
        }

        private void ConfigureModelInvoiceLifeCycleEvent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceLifeCycleEvent>().ToTable("CAAPS_INVOICE_LIFECYCLE_EVENTS");

            modelBuilder.Entity<InvoiceLifeCycleEvent>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.RecordID).HasColumnName("RECORDID");
                entity.Property(e => e.ClientApprovalDate).HasColumnName("CLIENT_APPROVAL_DATE");
                entity.Property(e => e.ExportDate).HasColumnName("EXPORT_DATE");
                entity.Property(e => e.PaymentNotificationDate).HasColumnName("PAYMENT_NOTFICATION_DATE");
                entity.Property(e => e.ReceivedDate).HasColumnName("RECEIVED_DATE");
                entity.Property(e => e.OwnershipTakenDate).HasColumnName("OWNERSHIP_TAKEN_DATE");
                entity.Property(e => e.ApprovalDate).HasColumnName("APPROVAL_DATE");
                entity.Property(e => e.ErpImportDate).HasColumnName("ERP_IMPORT_DATE");
                entity.Property(e => e.ArchivedDate).HasColumnName("ARCHIVED_DATE");
                entity.Property(e => e.RejectedDate).HasColumnName("REJECTED_DATE");
                entity.Property(e => e.ImportedDate).HasColumnName("IMPORTED_DATE");
                entity.Property(e => e.ApprovedDate).HasColumnName("APPROVED_DATE");
            });
        }

        private void ConfigureModelSystemOptions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemOption>().ToTable("SY_OPTIONS");

            modelBuilder.Entity<SystemOption>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.OptionName).HasColumnName("OPTIONNAME");
                entity.Property(e => e.OptionValue).HasColumnName("OPTIONVALUE");
                entity.Property(e => e.OptionMemo).HasColumnName("OPTIONMEMO");
            });
        }

        private void ConfigureModelFileLink(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileLink>().ToTable("FILELINKS");

            modelBuilder.Entity<FileLink>().HasKey(e => new { e.RecordId, e.FileId });

            modelBuilder.Entity<FileLink>(entity =>
            {
                entity.Property(e => e.RecordId).HasColumnName("RECORDID");
                entity.Property(e => e.FileId).HasColumnName("FILEID");
                entity.Property(e => e.FileIndex).HasColumnName("FILEINDEX");
                entity.Property(e => e.ManagedTableId).HasColumnName("MANAGEDTABLEID");
            });
        }

        private void ConfigureModelFileName(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileNameRecord>().ToTable("FILENAMES");

            modelBuilder.Entity<FileNameRecord>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.FileName).HasColumnName("FILENAME");
                entity.Property(e => e.FilePath).HasColumnName("FILEPATH");
            });
        }

        private void ConfigureModelS3DriveConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<S3DriveConfiguration>().ToTable("S3_DRIVE_CONFIGURATION");

            modelBuilder.Entity<S3DriveConfiguration>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.PhysicalDriveToken).HasColumnName("PHYSICAL_DRIVE_TOKEN");
                entity.Property(e => e.DefaultTimeoutInMinutes).HasColumnName("DEFAULT_TIMEOUT_IN_MINUTES");
                entity.Property(e => e.ResignBufferThresholdInMinutes).HasColumnName("RESIGN_BUFFER_THRESHOLD_IN_MINUTES");
                entity.Property(e => e.S3Bucket).HasColumnName("S3_BUCKET");
                entity.Property(e => e.S3Profile).HasColumnName("S3_PROFILE");
                entity.Property(e => e.S3Region).HasColumnName("S3_REGION");
                entity.Property(e => e.AWSCLIExePath).HasColumnName("AWS_CLI_EXE_PATH");
            });
        }

        private void ConfigureModelSundryPOGoodsAllocations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SundryPOGoodsAllocation>().ToTable("BRE_CAAPS_SUNDRY_PO_GOODS_ALLOCATION");

            modelBuilder.Entity<SundryPOGoodsAllocation>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.RecordID).HasColumnName("RECORDID");
                entity.Property(e => e.PoLineNumber).HasColumnName("PO_LINE_NUMBER");
                entity.Property(e => e.PoLineUOM).HasColumnName("PO_LINE_UOM");
                entity.Property(e => e.PoLineTaxCode).HasColumnName("PO_LINE_TAX_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.GrReceiptedValue).HasColumnName("GR_RECEIPTED_VALUE");
                entity.Property(e => e.GrReceiptedQty).HasColumnName("GR_RECEIPTED_QTY");
                entity.Property(e => e.GrNumber).HasColumnName("GR_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.GrReceivedDate).HasColumnName("GR_RECEIVED_DATE");
            });
        }

        private void ConfigureModelGLCode(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GLCodeDetails>().ToTable("BRE_CAAPS_GL_CODE_DETAILS");

            modelBuilder.Entity<GLCodeDetails>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE");
                entity.Property(e => e.EntityName).HasColumnName("ENTITY_NAME");
                entity.Property(e => e.GLCode).HasColumnName("GL_CODE");
                entity.Property(e => e.GLCodeDescription).HasColumnName("GL_CODE_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UpdatedByUser).HasColumnName("UPDATED_BY_USER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("BRE_CAAPS_PRODUCT_DETAILS");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.VendorBusinessNumber).HasColumnName("VENDOR_BUSINESS_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorName).HasColumnName("VENDOR_NAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorProductCode).HasColumnName("VENDOR_PRODUCT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorProductDescription).HasColumnName("VENDOR_PRODUCT_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.VendorUOM).HasColumnName("VENDOR_UOM").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoProductCode).HasColumnName("PO_PRODUCT_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoProductDescription).HasColumnName("PO_PRODUCT_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoUOM).HasColumnName("PO_UOM").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelTaxCode(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxCodeDetails>().ToTable("BRE_CAAPS_TAX_CODES");

            modelBuilder.Entity<TaxCodeDetails>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.TaxCode).HasColumnName("TAX_CODE");
                entity.Property(e => e.TaxRate).HasColumnName("TAX_RATE");
                entity.Property(e => e.TaxDescription).HasColumnName("TAX_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.UpdatedByUser).HasColumnName("UPDATED_BY_USER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.ProcessingCurrency).HasColumnName("PROCESSING_CURRENCY").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.DefaultTaxCodeFlag).HasColumnName("DEFAULT_TAX_CODE_FLAG");
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelClosedPurchaseOrder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClosedPurchaseOrder>().ToTable("BRE_CAAPS_CLOSED_PURCHASE_ORDERS");

            modelBuilder.Entity<ClosedPurchaseOrder>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.PoNumber).HasColumnName("PO_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoAmountTotal).HasColumnName("PO_AMOUNT_TOTAL").HasConversion(CustomValueConverters.NullableDecimalConverter());
                entity.Property(e => e.VendorCode).HasColumnName("VENDOR_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoRaisedDate).HasColumnName("PO_RAISED_DATE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.EntityCode).HasColumnName("ENTITY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoType).HasColumnName("PO_TYPE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoDate).HasColumnName("PO_DATE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoRaisedByUsername).HasColumnName("PO_RAISED_BY_USERNAME").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.JobNumber).HasColumnName("JOB_NUMBER").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.CurrencyCode).HasColumnName("CURRENCY_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoStatusCode).HasColumnName("PO_STATUS_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.PoOpenAmount).HasColumnName("PO_OPEN_AMOUNT").HasConversion(CustomValueConverters.NullableDecimalConverter());
                entity.Property(e => e.PayToVendorCode).HasColumnName("PAY_TO_VENDOR_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.SiteCode).HasColumnName("SITE_CODE").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureModelPaymentTerm(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentTerms>().ToTable("BRE_CAAPS_PAYMENT_TERMS");

            modelBuilder.Entity<PaymentTerms>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.PaymentTermsCode).HasColumnName("PAYMENT_TERMS_CODE");
                entity.Property(e => e.PaymentTermsDescription).HasColumnName("PAYMENT_TERMS_DESCRIPTION").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom01).HasColumnName("CUSTOM_01").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom02).HasColumnName("CUSTOM_02").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom03).HasColumnName("CUSTOM_03").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.Custom04).HasColumnName("CUSTOM_04").HasConversion(CustomValueConverters.NullableStringConverter());
                entity.Property(e => e.LastModifiedDateTime).HasColumnName("LAST_MODIFIED_DATETIME");
            });
        }

        private void ConfigureEntityRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrder>()
                        .HasMany(p => p.GoodsReceipts)
                        .WithOne(g => g.PurchaseOrder)
                        .HasForeignKey(g => new { g.PoNumber, g.PoLineNumber })
                        .HasPrincipalKey(p => new { p.PoNumber, p.LineNumber });

            modelBuilder.Entity<GoodsReceipt>()
                        .HasOne(g => g.PurchaseOrder)
                        .WithMany(p => p.GoodsReceipts)
                        .HasForeignKey(g => new { g.PoNumber, g.PoLineNumber })
                        .HasPrincipalKey(p => new { p.PoNumber, p.LineNumber });
        }
    }
}