using Hydro;

namespace HydroSales.Pages.Invoices.Common;

public class InvoiceLines : HydroView
{
    public List<InvoiceLineModel> Lines { get; set; }
    public string CurrencyCode { get; set; }
    public bool FocusLastLine { get; set; }
    public IInvoiceActions InvoiceActions { get; set; }
}