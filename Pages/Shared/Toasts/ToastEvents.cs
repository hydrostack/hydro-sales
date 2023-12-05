namespace HydroSales.Pages.Shared.Toasts;

public record ShowMessage(string Message, ToastType Type = ToastType.Success);
