using HydroSales.Database;
using HydroSales.Domain;
using Microsoft.EntityFrameworkCore;

namespace HydroSales.Pages.Shared.EditorTemplates;

public class CustomerSelectList(IDatabase database) : DynamicSelect
{
    public override string ItemPartial => GetViewPath("CustomerSelectItem");

    protected override async Task<IReadOnlyList<SelectItem>> GetItemsAsync()
    {
        var query = database.Query<Customer>();

        var phrase = Text?.Replace("%", "");

        if (!string.IsNullOrWhiteSpace(phrase))
        {
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{Text}%"));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new SelectItem(c.Id, c.Name, c.CountryCode, null))
            .ToListAsync();
    }

    protected override Task<string> FindValueByTextAsync(string text) =>
        database.Query<Customer>()
            .Where(c => c.Name == text)
            .Select(c => c.Id)
            .SingleOrDefaultAsync();

    protected override Task<string> GetTextFromValueAsync(string value) =>
        database.Query<Customer>(includeRemoved: true)
            .Where(c => c.Id == value)
            .Select(c => c.Name)
            .SingleOrDefaultAsync();
}