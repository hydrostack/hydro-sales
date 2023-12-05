using HydroSales.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static HydroSales.Utils.Calculator;
using static HydroSales.Utils.IdProvider;

namespace HydroSales.Domain;

public class InvoiceLine : IEntity
{
    private InvoiceLine()
    {
    }

    public string Id { get; private set; }
    public Invoice Invoice { get; private set; }
    public Product Product { get; private set; }
    public string ProductName { get; private set; }
    public string CurrencyCode { get; private set; }

    public decimal SalesTax { get; private set; }
    public decimal UnitPriceNet { get; private set; }

    public decimal Quantity { get; private set; }
    public string Unit { get; private set; }
    public decimal BaseValueNet { get; private set; }

    public decimal ValueNet { get; private set; }
    public decimal ValueTax { get; private set; }
    public decimal ValueGross { get; private set; }

    public static InvoiceLine Create(Invoice invoice, Data data)
    {
        var line = new InvoiceLine
        {
            Id = NewId(),
            Invoice = invoice,
            Product = data.Product,
            ProductName = data.Product.Name,
            CurrencyCode = data.CurrencyCode,
            SalesTax = data.SalesTax,
            UnitPriceNet = data.UnitPriceNet,
            Quantity = data.Quantity,
            Unit = data.Unit
        };

        Summarize(line);

        return line;
    }
    
    public void Update(Data data)
    {
        Product = data.Product;
        ProductName = data.Product.Name;
        CurrencyCode = data.CurrencyCode;
        SalesTax = data.SalesTax;
        UnitPriceNet = data.UnitPriceNet;
        Quantity = data.Quantity;
        Unit = data.Unit;

        Summarize(this);
    }

    private static void Summarize(InvoiceLine line)
    {
        line.BaseValueNet = CalculateValue(line.UnitPriceNet, line.Quantity);
        line.ValueNet = ConvertCurrency(line.BaseValueNet, line.CurrencyCode, line.Invoice.CurrencyCode);
        line.ValueTax = CalculateTax(line.ValueNet, line.SalesTax);
        line.ValueGross = CalculateGross(line.ValueNet, line.SalesTax);
    }
    
    public record Data(
        string Id,
        Product Product,
        string CurrencyCode,
        string Unit,
        decimal UnitPriceNet,
        decimal Quantity,
        decimal SalesTax
    );
}

public class InvoiceLineEntity : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasMaxLength(36);

        builder.HasOne(b => b.Product).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.Property(b => b.ProductName).HasMaxLength(100).IsRequired();
        builder.Property(b => b.CurrencyCode).HasMaxLength(3).IsRequired();

        builder.Property(b => b.Quantity).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.Unit).HasMaxLength(20).IsRequired();

        builder.Property(b => b.SalesTax).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.UnitPriceNet).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.BaseValueNet).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.ValueNet).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.ValueTax).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.ValueGross).HasPrecision(20, 3).IsRequired();
    }
}