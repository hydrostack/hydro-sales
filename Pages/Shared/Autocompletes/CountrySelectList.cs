using HydroSales.Authorization;
using HydroSales.Domain;

namespace HydroSales.Pages.Shared.Autocompletes;

[ComponentAuthorize]
public class CountrySelectList : DynamicSelect
{
    public override string ItemPartial => GetViewPath("CountrySelectItem");

    protected override IReadOnlyList<SelectItem> GetItems() =>
        CountryData.List
            .Where(c => string.IsNullOrWhiteSpace(Text) ||
                        string.Equals(c.Key, Text, StringComparison.OrdinalIgnoreCase)
                        || c.Value.Contains(Text, StringComparison.OrdinalIgnoreCase))
            .Select(c => new SelectItem(c.Key, c.Value))
            .ToList();

    protected override string FindValueByText(string text) =>
        CountryData.FindValue(text);

    protected override string GetTextFromValue(string value) =>
        CountryData.GetText(value);
}