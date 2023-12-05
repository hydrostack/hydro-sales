using Hydro;

namespace HydroSales.Pages.Shared.Toasts;

public class Toasts : HydroComponent
{
    public List<Toast> ToastsList { get; set; } = new();

    public Toasts()
    {
        Subscribe<ShowMessage>(Handle);
        Subscribe<UnhandledHydroError>(Handle);
    }

    private void Handle(ShowMessage data) =>
        ToastsList.Add(new Toast(
            Id: Guid.NewGuid().ToString("N"),
            Message: data.Message,
            Type: data.Type
        ));

    private void Handle(UnhandledHydroError data) =>
        ToastsList.Add(new Toast(
            Id: Guid.NewGuid().ToString("N"),
            Message: data.Message ?? "Unhand",
            Type: ToastType.Error
        ));

    public void Close(string id) =>
        ToastsList.RemoveAll(t => t.Id == id);

    public record Toast(string Id, string Message, ToastType Type);
}
