using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Models;

namespace Access.Configuration
{
    public abstract class BaseSettingEntityConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseSettingEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(m => m.Id)
                .UseOracleIdentityColumn()
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");

            builder.Property(m => m.Order)
                .HasColumnName("ORDER");

            builder.Property(m => m.Title)
                .HasColumnName("TITLE")
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(false);

            builder.Property(m => m.IsActive)
                .HasColumnName("IS_ACTIVE");

            builder.Property(m => m.IsDefault)
                .HasColumnName("IS_DEFAULT");

            builder.Property(m => m.UserCreated)
                .HasColumnName("CREATED_BY")
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false);

            builder.Property(m => m.DateCreated)
                .HasColumnName("DATE_CREATED")
                .HasColumnType("DATE");

            builder.Property(m => m.UserModified)
                .HasColumnName("MODIFIED_BY")
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(m => m.DateModified)
                .HasColumnName("DATE_MODIFIED")
                .IsRequired(false)
                .HasColumnType("DATE");
        }
    }
}
