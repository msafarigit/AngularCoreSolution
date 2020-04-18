using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Access.Configuration;
using Common.Models;

namespace Access.Context
{
    public class DataContext : BaseDbContext
    {
        public DataContext([NotNull] DbContextOptions<DataContext> options) : base(options)
        {
            UserName = "admin";
        }

        public DbSet<Region> Regions { get; private set; }

        public DbQuery<CustomerCallHistoryDto> CustomerCallHistories { get; set; }
        public DbQuery<InanHistoryDto> IncidentAnnouncementHistories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CustomerCallHistoryViewConfig());
            modelBuilder.ApplyConfiguration(new InanHistoryViewConfig());

            modelBuilder.ApplyConfiguration(new RegionEntityConfig());
        }
    }
}
