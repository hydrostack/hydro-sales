using HydroSales.Utils;

namespace HydroSales.Pages.Invoices.Common;

public interface IInvoiceSummaryModel
{
    List<TaxGroup> TaxGroups { get; }
    decimal ValueNet { get; }
    decimal ValueGross { get; }
    string CurrencyCode { get; }
}