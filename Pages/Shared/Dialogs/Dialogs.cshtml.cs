using Hydro;

namespace HydroSales.Pages.Shared.Dialogs;

public class Dialogs : HydroComponent
{
    public Dictionary<string, object> DialogsDictionary { get; set; }

    public Dialogs()
    {
        Subscribe<OpenDialog>(Handle);
        Subscribe<CloseDialog>(Handle);
    }

    public override void Mount() =>
        DialogsDictionary = new Dictionary<string, object>();

    private void Handle(OpenDialog dialog) =>
        DialogsDictionary.TryAdd(dialog.Name, dialog.Data);

    private void Handle(CloseDialog dialog) =>
        DialogsDictionary.Remove(dialog.Name);
}

public record EditDialogData(string Id);
