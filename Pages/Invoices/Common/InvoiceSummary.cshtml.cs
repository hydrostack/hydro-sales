using Hydro;
using HydroSales.Utils;

namespace HydroSales.Pages.Invoices.Common;

public class InvoiceSummary : HydroView
{
    public string CurrencyCode { get; set; }
    public decimal ValueGross { get; set; }
    public decimal ValueTax { get; set; }
    public decimal ValueNet { get; set; }
    public List<TaxGroup> TaxGroups { get; set; }
}