using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Models;

namespace Access.Configuration
{
    public class CustomerCallHistoryViewConfig : IQueryTypeConfiguration<CustomerCallHistoryDto>
    {
        public void Configure(QueryTypeBuilder<CustomerCallHistoryDto> builder)
        {
            builder.ToView("VW_INAN_CUSTOMER_CALL_HISTORY");

            builder.Property(p => p.TelephoneNumber).HasColumnName("TELEPHONE_NUMBER");
            builder.Property(p => p.CallDate).HasColumnName("CALL_DATE");
            builder.Property(p => p.CallerTelephone).HasColumnName("CALLER_TELEPHONE");
            builder.Property(p => p.CustomerFirstName).HasColumnName("CUSTOMER_FIRST_NAME");
            builder.Property(p => p.CustomerLastName).HasColumnName("CUSTOMER_LAST_NAME");
            builder.Property(p => p.CustomerMobileNumber).HasColumnName("CUSTOMER_MOBILE_NUMBER");
            builder.Property(p => p.CustomerTelNumber).HasColumnName("CUSTOMER_TELEPHONE");
            builder.Property(p => p.CallTypeId).HasColumnName("CALL_TYPE_ID");
            builder.Property(p => p.BillId).HasColumnName("BILL_ID");
            builder.Property(p => p.IdentityCode).HasColumnName("IDENTITY_CODE");
            builder.Property(p => p.SubscribeCode).HasColumnName("SUBSCRIBE_CODE");
            builder.Property(p => p.DigitalCode).HasColumnName("DIGITAL_CODE");
            builder.Property(p => p.CustomerAddress).HasColumnName("CUSTOMER_ADDRESS");
            builder.Property(p => p.CustomerPostalCode).HasColumnName("CUSTOMER_POSTAL_CODE");
            builder.Property(p => p.IncidentDate).HasColumnName("INCIDENT_DATE");
            builder.Property(p => p.IncidentTypeId).HasColumnName("INCIDENT_TYPE_ID");
            builder.Property(p => p.RegionId).HasColumnName("REGION_ID");
            builder.Property(p => p.DistrictId).HasColumnName("DISTRICT_ID");
            builder.Property(p => p.PoweroffDescription).HasColumnName("POWEROFF_DESCRIPTION");
            builder.Property(p => p.PoweroffReasonId).HasColumnName("POWEROFF_REASON_ID");
            builder.Property(p => p.NetworkDisadvantageDescription).HasColumnName("NETWORK_DISADVANTAGE_DESCRIPTION");
            builder.Property(p => p.NetworkDisadvantageAddress).HasColumnName("NETWORK_DISADVANTAGE_ADDRESS");
            builder.Property(p => p.NetworkDisadvantageReasonId).HasColumnName("NETWORK_DISADVANTAGE_REASON_ID");
            builder.Property(p => p.OverallPassageLightingDescription).HasColumnName("OVERALL_PASSAGE_DESCRIPTION");
            builder.Property(p => p.OverallPassageLightingAddress).HasColumnName("OVERALL_PASSAGE_ADDRESS");
            builder.Property(p => p.OverallPassageLightingReasonId).HasColumnName("OVERALL_PASSAGE_REASON_ID");
            builder.Property(p => p.PartialPassageLightingDescription).HasColumnName("PARTIAL_PASSAGE_DESCRIPTION");
            builder.Property(p => p.PartialPassageLightingAddress).HasColumnName("PARTIAL_PASSAGE_ADDRESS");
            builder.Property(p => p.PartialPassageLightingReasonId).HasColumnName("PARTIAL_PASSAGE_REASON_ID");
            builder.Property(p => p.IrrelevantCallDescription).HasColumnName("IRRELEVANT_CALL_DESCRIPTION");
            builder.Property(p => p.IrrelevantCallReasonId).HasColumnName("IRRELEVANT_CALL_REASON_ID");
        }
    }
}
