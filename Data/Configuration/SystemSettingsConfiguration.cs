using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using erpv0._1.Models;

namespace erpv0._1.Data.Configuration
{
    public class SystemSettingsConfiguration : IEntityTypeConfiguration<SystemSettings>
    {
        public void Configure(EntityTypeBuilder<SystemSettings> builder)
        {
            builder.ToTable("SystemSettings");

            builder.HasKey(s => s.SettingId);

            // Fix for DefaultTaxRate decimal precision
            builder.Property(s => s.DefaultTaxRate)
                .HasPrecision(5, 2) // 5 total digits, 2 decimal places
                .HasDefaultValue(15.00m)
                .IsRequired();

            builder.Property(s => s.SystemName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.LogoPath)
                .HasMaxLength(500);

            builder.Property(s => s.DefaultLanguage)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(s => s.TimeZone)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.DateFormat)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Currency)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(s => s.CurrencyFormat)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.SystemEmail)
                .HasMaxLength(256);

            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(s => s.CompanyAddress)
                .HasMaxLength(500);

            builder.Property(s => s.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(s => s.UpdatedBy)
                .HasMaxLength(256);

            // Default values
            builder.Property(s => s.EnableNotifications)
                .HasDefaultValue(true);

            builder.Property(s => s.LowStockThreshold)
                .HasDefaultValue(10);

            builder.Property(s => s.EnableAuditLog)
                .HasDefaultValue(true);

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Indexes
            builder.HasIndex(s => s.DefaultLanguage);
            builder.HasIndex(s => s.SystemName)
                .IsUnique();
        }
    }
}