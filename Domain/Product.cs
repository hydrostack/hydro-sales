using HydroSales.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static HydroSales.Utils.Calculator;
using static HydroSales.Utils.IdProvider;

namespace HydroSales.Domain;

public class Product : IEntity, IRemovable, ITenant
{
    private Product()
    {
    }

    public string Id { get; private set; }
    public User User { get; private set; }
    public bool IsRemoved { get; private set; }

    public string Name { get; private set; }
    public string Code { get; private set; }
    public decimal PriceNet { get; private set; }
    public decimal SalesTax { get; private set; }
    public decimal PriceGross { get; private set; }
    public string CurrencyCode { get; private set; }

    public static Product Create(
        User user,
        string name,
        string code,
        decimal priceNet,
        decimal salesTax,
        string currencyCode
    ) => new()
    {
        Id = NewId(),
        User = user,
        Name = name,
        Code = code,
        PriceNet = priceNet,
        SalesTax = salesTax,
        PriceGross = CalculateGross(priceNet, salesTax),
        CurrencyCode = currencyCode
    };

    public void Edit(
        string name,
        string code,
        decimal priceNet,
        decimal salesTax,
        string currencyCode
    )
    {
        Name = name;
        Code = code;
        PriceNet = priceNet;
        SalesTax = salesTax;
        PriceGross = CalculateGross(priceNet, salesTax);
        CurrencyCode = currencyCode;
    }

    public void Remove() =>
        IsRemoved = true;
}

public class ProductEntity : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasMaxLength(36);
        builder.HasOne(b => b.User).WithMany().OnDelete(DeleteBehavior.Restrict);
        builder.Property(b => b.IsRemoved);

        builder.Property(b => b.Name).HasMaxLength(100).IsRequired().UseCollation("nocase");
        builder.Property(b => b.Code).HasMaxLength(50).UseCollation("nocase");
        builder.Property(b => b.PriceNet).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.PriceGross).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.SalesTax).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.CurrencyCode).HasMaxLength(3).IsRequired();
    }
}