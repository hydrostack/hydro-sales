using Hydro;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HydroSales.Pages.Shared.Fields;

public class FieldCustomer : HydroView
{
    public ModelExpression Field { get; set; }
    public bool AutoFocus { get; set; }
    public string Label { get; set; }
}