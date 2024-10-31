namespace HydroSales.Pages.Invoices.Common;

public interface IInvoiceActions
{
    void AddLine();
    void RemoveLine(int index);
}