namespace HydroSales.Pages.Shared.Dialogs;

public record OpenDialog(string Name, object Data = null);
public record CloseDialog(string Name);