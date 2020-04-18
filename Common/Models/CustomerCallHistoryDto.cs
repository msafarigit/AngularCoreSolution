using System;

namespace Common.Models
{
    public class CustomerCallHistoryDto
    {
        public string TelephoneNumber { get; set; } //only for search
        public DateTime CallDate { get; set; }
        public string CallerTelephone { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerTelNumber { get; set; }
        public byte CallTypeId { get; set; }
        public string BillId { get; set; }
        public string IdentityCode { get; set; }
        public string SubscribeCode { get; set; }
        public string DigitalCode { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPostalCode { get; set; }
        public DateTime IncidentDate { get; set; }
        public byte IncidentTypeId { get; set; } 
        public long RegionId { get; set; }
        public long DistrictId { get; set; }
        public string PoweroffDescription { get; set; }
        public long PoweroffReasonId { get; set; }
        public string NetworkDisadvantageDescription { get; set; }
        public string NetworkDisadvantageAddress { get; set; }
        public long NetworkDisadvantageReasonId { get; set; }
        public string OverallPassageLightingDescription { get; set; }
        public string OverallPassageLightingAddress { get; set; }
        public long OverallPassageLightingReasonId { get; set; }
        public string PartialPassageLightingDescription { get; set; }
        public string PartialPassageLightingAddress { get; set; }
        public long PartialPassageLightingReasonId { get; set; }
        public string IrrelevantCallDescription { get; set; }
        public long IrrelevantCallReasonId { get; set; }
    }
}
