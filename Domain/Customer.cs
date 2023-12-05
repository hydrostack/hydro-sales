using HydroSales.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static HydroSales.Utils.IdProvider;

namespace HydroSales.Domain;

public class Customer : IEntity, IRemovable, ITenant
{
    private Customer()
    {
    }

    public string Id { get; private set; }
    public User User { get; private set; }
    public bool IsRemoved { get; private set; }

    public string Name { get; private set; }
    public string TaxId { get; private set; }
    public string CurrencyCode { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string CountryCode { get; private set; }
    public int PaymentTerms { get; private set; }

    public static Customer Create(
        User user,
        string name,
        string taxId,
        string currencyCode,
        string address,
        string city,
        string countryCode,
        int paymentTerms
    ) => new()
    {
        Id = NewId(),
        User = user,
        Name = name,
        TaxId = taxId,
        CurrencyCode = currencyCode,
        Address = address,
        City = city,
        CountryCode = countryCode,
        PaymentTerms = paymentTerms
    };

    public void Edit(
        string name,
        string taxId,
        string currencyCode,
        string address,
        string city,
        string countryCode,
        int paymentTerms
    )
    {
        Name = name;
        TaxId = taxId;
        CurrencyCode = currencyCode;
        Address = address;
        City = city;
        CountryCode = countryCode;
        PaymentTerms = paymentTerms;
    }

    public void Remove() =>
        IsRemoved = true;
}

public class CustomerEntity : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasMaxLength(36);
        builder.HasOne(b => b.User).WithMany().OnDelete(DeleteBehavior.Restrict);
        builder.Property(b => b.IsRemoved);

        builder.Property(b => b.Name).HasMaxLength(100).IsRequired().UseCollation("nocase");
        builder.Property(b => b.TaxId).HasMaxLength(20).IsRequired();
        builder.Property(b => b.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(b => b.City).HasMaxLength(255).IsRequired();
        builder.Property(b => b.Address).HasMaxLength(255).IsRequired();
        builder.Property(b => b.CountryCode).HasMaxLength(2).IsRequired();
        builder.Property(b => b.PaymentTerms).IsRequired();
    }
}