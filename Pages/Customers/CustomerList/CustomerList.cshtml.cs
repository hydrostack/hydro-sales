using System.ComponentModel.DataAnnotations;
using Hydro;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HydroSales.Pages.Customers.CustomerList;

[ComponentAuthorize]
public class CustomerList(IDatabase database) : HydroComponent
{
    public string SearchPhrase { get; set; }

    public Cache<Task<List<Customer>>> Customers => Cache(async () =>
    {
        var query = database.Query<Customer>();

        if (!string.IsNullOrWhiteSpace(SearchPhrase))
        {
            query = query.Where(p => p.Name.Contains(SearchPhrase));
        }

        return await query.ToListAsync();
    });

    [SkipOutput]
    public void Add() =>
        Location(Url.Page("/Customers/Add"));

    [SkipOutput]
    public void Edit(string id) =>
        Location(Url.Page("/Customers/Edit", new { id }));

    public void ClearSearch() =>
        SearchPhrase = null;
}