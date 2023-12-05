using HydroSales.Database;
using HydroSales.Domain;
using Microsoft.EntityFrameworkCore;

namespace HydroSales.Pages.Shared.EditorTemplates;

public class ProductSelectList(IDatabase database) : DynamicSelect
{
    public override string ItemPartial => GetViewPath("ProductSelectItem");

    protected override async Task<IReadOnlyList<SelectItem>> GetItemsAsync()
    {
        var query = database.Query<Product>();

        var phrase = Text?.Replace("%", "");

        if (!string.IsNullOrWhiteSpace(phrase))
        {
            query = query.Where(c => c.Code == Text || EF.Functions.Like(c.Name, $"%{Text}%"));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new SelectItem(c.Id, c.Name, c.Code, new ProductMetadata(c.Code, c.PriceNet, c.SalesTax)))
            .ToListAsync();
    }

    protected override Task<string> FindValueByTextAsync(string text) =>
        database.Query<Product>()
            .Where(c => c.Code == text || c.Name == text)
            .Select(c => c.Id)
            .SingleOrDefaultAsync();

    protected override async Task<string> GetTextFromValueAsync(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : await database.Query<Product>(id: value, includeRemoved: true)
                .Select(c => c.Name)
                .SingleOrDefaultAsync();

    public record ProductMetadata(string Code, decimal PriceNet, decimal SalesTax);
}