using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Models;

namespace Access.Configuration
{
    public class RegionEntityConfig : BaseSettingEntityConfig<Region>
    {
        public override void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.ToTable("TB_SETT_REGION");

            builder.Property(m => m.CompanyId)
                .HasColumnName("COMPANY_ID");

            base.Configure(builder);
        }
    }
}
