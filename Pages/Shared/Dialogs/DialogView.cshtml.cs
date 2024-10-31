using System.Linq.Expressions;
using Hydro;

namespace HydroSales.Pages.Shared.Dialogs;

public class DialogView : HydroView
{
    public Expression<Action> OnClose { get; set; }
}