using HydroSales.Domain;

namespace HydroSales.Pages.Shared.EditorTemplates;

public class CurrencySelectList : DynamicSelect
{
    protected override IReadOnlyList<SelectItem> GetItems() =>
        CurrencyData.List
            .Where(c => string.IsNullOrWhiteSpace(Text)
                        || c.Key.StartsWith(Text, StringComparison.OrdinalIgnoreCase))
            .Select(c => new SelectItem(c.Key, c.Key))
            .ToList();

    protected override string FindValueByText(string text) =>
        CurrencyData.FindKey(text);

    protected override string GetTextFromValue(string value) =>
        CurrencyData.GetText(value);
}