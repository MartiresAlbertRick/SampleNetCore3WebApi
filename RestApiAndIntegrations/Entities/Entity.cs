using System;

namespace AD.CAAPS.Entities
{
    public partial class Entity
    {
        public int ID { get; set; }
        public string EntityCode { get; set; }
        public string EntityCodeEquivalent { get; set; }
        public string EntityName { get; set; }
        public string EntityNameEquivalent { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string BusinessUnitCode { get; set; }
        public string BusinessUnitName { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string DefaultContactUserName { get; set; }
        public string EmailSignOff { get; set; }
        public string ReplyToAddress { get; set; }
        public string BccAddress { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string ClientCaapsProcessingAddress { get; set; }
        public string ClientAPQueriesAddress { get; set; }
        public string ProcessingCurrency { get; set; }
        public string UpdatedByUser { get; set; }
        public string ReferenceAddress { get; set; }
        public string EntityBusinessNumber { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}