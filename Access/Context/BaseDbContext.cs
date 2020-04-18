using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Common.Models;

namespace Access.Context
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext([NotNull] DbContextOptions options)
            : base(options) { }

        protected string UserName { get; set; }

        public override int SaveChanges()
        {
            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IEntityCreator || entry.Entity is IEntityModifier)
                {
                    if (entry.State == EntityState.Added)
                    {
                        IEntityCreator entity = (IEntityCreator)entry.Entity;
                        entity.UserCreated = UserName;
                        entity.DateCreated = DateTime.Now;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        IEntityModifier entity = (IEntityModifier)entry.Entity;
                        entity.UserModified = UserName;
                        entity.DateModified = DateTime.Now;
                    }
                }
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("EDS_NAB");
        }
    }

}
