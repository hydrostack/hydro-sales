using System.ComponentModel.DataAnnotations;
using Hydro;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HydroSales.Pages.Products.ProductList;

[ComponentAuthorize]
public class ProductList(IDatabase database) : HydroComponent
{
    [MaxLength(50)]
    public string SearchPhrase { get; set; }

    public Cache<Task<List<Product>>> Products => Cache(async () =>
    {
        var query = database.Query<Product>();

        if (!string.IsNullOrWhiteSpace(SearchPhrase))
        {
            query = query.Where(p => p.Name.Contains(SearchPhrase));
        }

        return await query.ToListAsync();
    });

    [SkipOutput]
    public void Add() =>
        Location(Url.Page("/Products/Add"));

    [SkipOutput]
    public void Edit(string id) =>
        Location(Url.Page("/Products/Edit", new { id }));

    public void ClearSearch() =>
        SearchPhrase = null;
}