using HydroSales.Database;
using HydroSales.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static HydroSales.Utils.IdProvider;

namespace HydroSales.Domain;

public class Invoice : IEntity, ITenant
{
    public string Id { get; private set; }
    public User User { get; private set; }
    public bool IsRemoved { get; private set; }

    public string Number { get; private set; }
    public DateTime CreationDate { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public int PaymentTerms { get; private set; }
    public string Remarks { get; private set; }

    public Customer Customer { get; private set; }

    public decimal ValueNet { get; private set; }
    public decimal ValueTax { get; private set; }
    public decimal ValueGross { get; private set; }

    public string CurrencyCode { get; private set; }

    public IReadOnlyList<InvoiceLine> Lines { get; private set; }

    public static Invoice Create(
        User user,
        Customer customer,
        string currencyCode,
        IEnumerable<InvoiceLine.Data> lines,
        DateTime issueDate,
        DateTime dueDate,
        int paymentTerms,
        string remarks
    )
    {
        var invoice = new Invoice
        {
            Id = NewId(),
            User = user,
            Number = GenerateNumber(user),
            CurrencyCode = currencyCode,
            Customer = customer,
            CreationDate = DateTime.UtcNow,
            IssueDate = issueDate,
            DueDate = dueDate,
            PaymentTerms = paymentTerms,
            Remarks = remarks
        };

        invoice.Lines = lines
            .Select(line => InvoiceLine.Create(invoice: invoice, data: line))
            .ToList();

        Summarize(invoice);

        return invoice;
    }

    public void Update(
        Customer customer,
        string currencyCode,
        IEnumerable<InvoiceLine.Data> lines,
        DateTime issueDate,
        DateTime dueDate,
        int paymentTerms,
        string remarks
    )
    {
        CurrencyCode = currencyCode;
        Customer = customer;
        IssueDate = issueDate;
        DueDate = dueDate;
        PaymentTerms = paymentTerms;
        Remarks = remarks;
        Lines = lines.Select(lineData =>
        {
            var existingLine = Lines.FirstOrDefault(line => line.Id == lineData.Id);

            if (existingLine == null)
            {
                return InvoiceLine.Create(invoice: this, data: lineData);
            }

            existingLine.Update(data: lineData);
            return existingLine;
        }).ToList();

        Summarize(this);
    }

    private static void Summarize(Invoice invoice)
    {
        var taxGroups = Calculator.GroupBySalesTax(invoice.Lines.Select(l => (l.SalesTax, l.ValueNet))).ToList();
        invoice.ValueNet = taxGroups.Sum(t => t.ValueNet);
        invoice.ValueTax = taxGroups.Sum(t => t.ValueTax);
        invoice.ValueGross = taxGroups.Sum(t => t.ValueGross);
    }

    private static string GenerateNumber(User user)
    {
        var nextNumber = user.Settings.BookInvoiceNumber();
        return $"INV/{nextNumber:00000}";
    }
}

public class InvoiceEntity : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasMaxLength(36);
        builder.HasOne(b => b.User).WithMany().OnDelete(DeleteBehavior.Restrict);

        builder.Property(b => b.Number).HasMaxLength(30).UseCollation("nocase");
        builder.Property(b => b.IssueDate).IsRequired();
        builder.Property(b => b.CreationDate).IsRequired();
        builder.Property(b => b.DueDate).IsRequired();
        builder.Property(b => b.PaymentTerms).IsRequired();
        builder.Property(b => b.Remarks);
        builder.HasOne(b => b.Customer).WithMany().OnDelete(DeleteBehavior.Restrict);
        builder.Property(b => b.CurrencyCode).HasMaxLength(3).IsRequired();

        builder.Property(b => b.ValueNet).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.ValueTax).HasPrecision(20, 3).IsRequired();
        builder.Property(b => b.ValueGross).HasPrecision(20, 3).IsRequired();
    }
}