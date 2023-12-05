namespace HydroSales.Pages.Invoices.Common;

public interface IInvoiceLinesModel
{
    List<InvoiceLineModel> Lines { get; }
    string CurrencyCode { get; }
    bool FocusLastLine { get; set; }
    void AddLine();
    void RemoveLine(int index);
}