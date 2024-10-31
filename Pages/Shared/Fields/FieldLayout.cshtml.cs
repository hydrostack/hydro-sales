using Hydro;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HydroSales.Pages.Shared.Fields;

public class FieldLayout : HydroView
{
    public string Name { get; set; }
    public string Label { get; set; }
    public string Error { get; set; }
}