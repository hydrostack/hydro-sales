using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HydroSales.Pages.Shared.Fields;

public static class FieldUtils
{
    public static FieldData GetInfo(this ModelExpression modelExpression, ModelStateDictionary modelState, string label = null)
    {
        var name = modelExpression.Name;
        var value = modelExpression.Model;
        var finalLabel = label ?? modelExpression.ModelExplorer.Metadata.DisplayName ?? "";
        var error = modelState.TryGetValue(name, out var state) && state.Errors.Any()
            ? state.Errors[0].ErrorMessage
            : null;
        var isValid = error == null;
        
        return new FieldData(name, value, finalLabel, isValid, error);
    }

    public record FieldData(string Name, object Value, string Label, bool IsValid, string Error);
}