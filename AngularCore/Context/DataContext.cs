using Edsab.Common.DataAccessLayer.EntityFramework;
using Edsab.Nab.DataAccess.EntityFramework.IncidentAnnouncement;
using Edsab.Nab.DataAccess.EntityFramework.Operator;
using Edsab.Nab.DataAccess.EntityFramework.Setting.General;
using Edsab.Nab.DataAccess.EntityFramework.Setting.IncidentAnnouncement;
using Edsab.Nab.Dto.IncidentAnnouncement;
using Edsab.Nab.Entity.IncidentAnnouncement;
using Edsab.Nab.Entity.Operator;
using Edsab.Nab.Entity.Setting.General;
using Edsab.Nab.Entity.Setting.IncidentAnnouncement;
using Edsab.Nima.Common;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Edsab.Nab.DataAccess.EntityFramework.Context
{
    public class DataContext : BaseDbContext
    {
        public DataContext([NotNull] DbContextOptions<DataContext> options, [NotNull] SessionManager sessionManager) : base(options)
        {
            UserName = sessionManager.GetUserName();
        }

        public DbSet<Region> Regions { get; private set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerCall> CustomerCalls { get; set; }
        public DbSet<CallTelephoneNumber> CallTelephoneNumbers { get; set; }
        public DbSet<IncidentAnnouncementEntity> IncidentAnnouncements { get; set; }
        public DbSet<IrrelevantCallTo121> IrrelevantCallsTo121 { get; set; }
        public DbSet<PowerOff> PowerOffs { get; set; }
        public DbSet<NetworkDisadvantage> NetworkDisadvantages { get; set; }
        public DbSet<PartialPassageLighting> PartialPassageLightings { get; set; }
        public DbSet<OverallPassageLighting> OverallPassageLightings { get; set; }
        public DbSet<OperatorEntity> Operators { get; set; }
        public DbSet<OperatorLogin> OperatorLogins { get; set; }
        public DbSet<CustomerCallTelephoneNumber> CustomerCallTelephoneNumbers { get; set; }
        public DbQuery<CustomerCallHistoryDto> CustomerCallHistories { get; set; }
        public DbQuery<InanHistoryDto> IncidentAnnouncementHistories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CustomerEntityConfig());
            modelBuilder.ApplyConfiguration(new CustomerCallEntityConfig());
            modelBuilder.ApplyConfiguration(new CallTelephoneNumberEntityConfig());
            modelBuilder.ApplyConfiguration(new IncidentAnnouncementEntityConfig());
            modelBuilder.ApplyConfiguration(new IrrelevantCallTo121EntityConfig());
            modelBuilder.ApplyConfiguration(new PowerOffEntityConfig());
            modelBuilder.ApplyConfiguration(new NetworkDisadvantageEntityConfig());
            modelBuilder.ApplyConfiguration(new PartialPassageLightingEntityConfig());
            modelBuilder.ApplyConfiguration(new OverallPassageLightingEntityConfig());
            modelBuilder.ApplyConfiguration(new OperatorEntityConfig());
            modelBuilder.ApplyConfiguration(new OperatorLoginEntityConfig());
            modelBuilder.ApplyConfiguration(new CustomerCallTelephoneNumberEntityConfig());
            modelBuilder.ApplyConfiguration(new CustomerCallHistoryViewConfig());
            modelBuilder.ApplyConfiguration(new InanHistoryViewConfig());

            modelBuilder.ApplyConfiguration(new RegionEntityConfig());
        }
    }
}
