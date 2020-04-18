using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Models;

namespace Access.Configuration
{
    public class InanHistoryViewConfig : IQueryTypeConfiguration<InanHistoryDto>
    {
        public void Configure(QueryTypeBuilder<InanHistoryDto> builder)
        {
            builder.ToView("VW_INAN_HISTORY");

            builder.Property(p => p.IncidentDate)
                .HasColumnName("INCIDENT_DATE");

            builder.Property(p => p.IncidentTypeId)
                .HasColumnName("INCIDENT_TYPE_ID");

            builder.Property(p => p.RegionId)
                .HasColumnName("REGION_ID");

            builder.Property(p => p.DistrictId)
                .HasColumnName("DISTRICT_ID");

            builder.Property(p => p.Address)
                .HasColumnName("ADDRESS");

            builder.Property(p => p.MainReason)
                .HasColumnName("MAIN_REASON");

            builder.Property(p => p.IncidentAnnouncementId)
                .HasColumnName("INAN_ID");

            builder.Property(p => p.ChildId)
                .HasColumnName("CHILD_ID");

            builder.Property(p => p.ChildTypeId)
                .HasColumnName("CHILD_TYPE_ID");
        }
    }
}
