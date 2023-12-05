using HydroSales.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HydroSales.Domain;

public class UserSettings : IEntity, ITenant
{
    private UserSettings()
    {
    }

    public string Id { get; private set; }
    public User User { get; private set; }
    public int RecentInvoiceNumber { get; private set; }
    public string DefaultCurrencyCode { get; private set; }

    public static UserSettings Create() => new()
    {
        DefaultCurrencyCode = "USD"
    };

    public int BookInvoiceNumber()
    {
        RecentInvoiceNumber++; // More assurance is needed
        return RecentInvoiceNumber;
    }
}

public class UserSettingsEntity : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasMaxLength(36);
        builder.HasOne(b => b.User).WithOne(s => s.Settings)
            .HasForeignKey<UserSettings>(b => b.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(b => b.DefaultCurrencyCode).HasMaxLength(3).IsRequired();
    }
}