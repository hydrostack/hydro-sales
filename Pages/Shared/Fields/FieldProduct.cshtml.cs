using Hydro;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HydroSales.Pages.Shared.Fields;

public class FieldProduct : HydroView
{
    public ModelExpression Field { get; set; }
    public string Label { get; set; }
}