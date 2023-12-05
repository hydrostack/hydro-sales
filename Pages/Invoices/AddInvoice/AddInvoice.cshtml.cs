using System.ComponentModel.DataAnnotations;
using Hydro;
using Hydro.Utils;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using HydroSales.Pages.Invoices.Common;
using HydroSales.Pages.Shared.Toasts;
using HydroSales.Utils;
using HydroSales.Utils.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hydro.Scope;
using static HydroSales.Utils.Calculator;

namespace HydroSales.Pages.Invoices.AddInvoice;

[ComponentAuthorize]
public class AddInvoice(IDatabase database) : HydroComponent, IInvoiceLinesModel, IInvoiceSummaryModel
{
    [Display(Name = "Customer"), Required]
    public string CustomerId { get; set; }

    [Display(Name = "Issue date"), Required]
    public DateTime IssueDate { get; set; }

    [Display(Name = "Payment terms"), Required]
    public int PaymentTerms { get; set; }

    [Display(Name = "Due date"), Required]
    public DateTime DueDate { get; set; }

    [Display(Name = "Currency"), Required]
    public string CurrencyCode { get; set; }

    [Display(Name = "Remarks")]
    public string Remarks { get; set; }

    [Display(Name = "Lines"), Required, ValidateCollection]
    public List<InvoiceLineModel> Lines { get; set; }

    [Transient]
    public bool FocusLastLine { get; set; }

    public decimal ValueGross { get; set; }

    public decimal ValueTax { get; set; }

    public decimal ValueNet { get; set; }

    public List<TaxGroup> TaxGroups { get; set; }

    public override async Task MountAsync()
    {
        var userSettings = await database.Query<UserSettings>().SingleAsync();

        CurrencyCode = userSettings.DefaultCurrencyCode;
        PaymentTerms = 30;
        IssueDate = DateTime.Today;
        DueDate = DateTime.Today;
        Lines =
        [
            new() { Quantity = 1, CurrencyCode = userSettings.DefaultCurrencyCode, Unit = "pc" }
        ];
        Summarize();
    }

    public async Task Save()
    {
        if (!Validate())
        {
            return;
        }

        var invoice = Invoice.Create(
            user: await database.GetCurrentUser(),
            customer: await database.Query<Customer>(CustomerId).SingleAsync(),
            currencyCode: CurrencyCode,
            lines: await Lines.Select(async input => new InvoiceLine.Data(
                Id: null,
                Product: await database.Query<Product>(input.ProductId).SingleAsync(),
                CurrencyCode: input.CurrencyCode,
                Unit: input.Unit,
                UnitPriceNet: input.UnitPriceNet,
                Quantity: input.Quantity,
                SalesTax: input.SalesTax
            )).ToListAsync(),
            issueDate: IssueDate,
            dueDate: DueDate,
            paymentTerms: PaymentTerms,
            remarks: Remarks
        );

        await database.AddAsync(invoice);
        await database.SaveChangesAsync();

        Back();
        Dispatch(new ShowMessage("Invoice has been added"), Global);
    }

    public void AddLine()
    {
        Lines.Add(new() { Quantity = 1, CurrencyCode = CurrencyCode, Unit = "pc" });
        FocusLastLine = true;
        Summarize();
    }

    public void RemoveLine(int index)
    {
        Lines.RemoveAt(index);
        Summarize();
    }

    public override async Task BindAsync(PropertyPath property, object value)
    {
        switch (property.Name)
        {
            case nameof(CustomerId):
                await BindCustomerId((string)value);
                break;
            case nameof(PaymentTerms):
                BindPaymentTerms((int)value);
                break;
            case nameof(Lines):
                await BindLine(property.GetIndex(), property.Child, value);
                break;
            case nameof(CurrencyCode):
                BindCurrencyCode((string)value);
                break;
        }
    }

    private async Task BindLine(int index, PropertyPath property, object value)
    {
        switch (property.Name)
        {
            case nameof(InvoiceLineModel.ProductId):
                await BindLineProductId(index, (string)value);
                break;
        }

        Summarize();
    }

    private async Task BindLineProductId(int index, string productId)
    {
        if (productId == null)
        {
            return;
        }

        var product = await database.Query<Product>(productId, includeRemoved: true)
            .Select(p => new { p.PriceNet, p.SalesTax })
            .SingleAsync();

        Lines[index].UnitPriceNet = product.PriceNet;
        Lines[index].SalesTax = product.SalesTax;
    }

    private void BindCurrencyCode(string currencyCode) =>
        Summarize();

    private void Summarize()
    {
        foreach (var line in Lines)
        {
            line.BaseValueNet = CalculateValue(line.UnitPriceNet, line.Quantity);
            line.ValueNet = ConvertCurrency(line.BaseValueNet, line.CurrencyCode, CurrencyCode);
            line.ValueGross = CalculateGross(line.ValueNet, line.SalesTax);
        }

        TaxGroups = GroupBySalesTax(Lines.Select(l => (l.SalesTax, l.ValueNet))).ToList();
        ValueNet = TaxGroups.Sum(t => t.ValueNet);
        ValueTax = TaxGroups.Sum(t => t.ValueTax);
        ValueGross = TaxGroups.Sum(t => t.ValueGross);
    }

    private void BindPaymentTerms(int paymentTerms) =>
        DueDate = IssueDate.AddDays(paymentTerms);

    private async Task BindCustomerId(string customerId)
    {
        if (customerId == null)
        {
            return;
        }

        var customer = await database.Query<Customer>()
            .Where(c => c.Id == customerId)
            .Select(c => new { c.PaymentTerms, c.CurrencyCode })
            .SingleAsync();

        PaymentTerms = customer.PaymentTerms;
        DueDate = IssueDate.AddDays(customer.PaymentTerms);
        CurrencyCode = customer.CurrencyCode;
    }

    [SkipOutput]
    public void Back() =>
        Location(Url.Page("/Invoices/Index"));
}