namespace HydroSales.Pages.Shared.Dialogs;

public class DialogOptions
{
    public string Title { get; set; } = "Dialog";
    public string HydroSubmitAction { get; set; } = "Save";
    public string HydroCloseAction { get; set; } = "Close";
    public string SubmitLabel { get; set; } = "Save changes";
    public string CloseLabel { get; set; } = "Close";
    public bool CloseOnEscape { get; set; } = false;
}
