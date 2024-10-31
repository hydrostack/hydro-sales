using Hydro;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HydroSales.Pages.Shared.Fields;

public class FieldText : HydroView
{
    public ModelExpression Field { get; set; }
    public string Align { get; set; }
    public bool AutoFocus { get; set; }
    public int? Rows { get; set; }
    public string Label { get; set; }
}