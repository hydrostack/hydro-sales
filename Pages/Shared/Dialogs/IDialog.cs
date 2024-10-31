namespace HydroSales.Pages.Shared.Dialogs;

public interface IDialog;

public interface IDialog<TData>
{
    public TData DialogData { get; set; }
}