using System.ComponentModel.DataAnnotations;
using Hydro;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HydroSales.Pages.Invoices.InvoiceList;

[ComponentAuthorize]
public class InvoiceList(IDatabase database) : HydroComponent
{
    [MaxLength(50)]
    public string SearchPhrase { get; set; }

    public Cache<Task<List<Item>>> Invoices => Cache(async () =>
    {
        var query = database.Query<Invoice>();

        var phrase = SearchPhrase?.Replace("%", "");

        if (!string.IsNullOrWhiteSpace(phrase))
        {
            query = query.Where(p => 
                EF.Functions.Like(p.Number, $"%{phrase}%")
                || EF.Functions.Like(p.Customer.Name, $"%{phrase}%"));
        }

        query = query.OrderByDescending(p => p.CreationDate);
        
        return await query
            .Select(i => new Item
            {
                Id = i.Id,
                Number = i.Number,
                IssueDate = i.IssueDate,
                CustomerName = i.Customer.Name,
                CurrencyCode = i.CurrencyCode,
                ValueNet = i.ValueNet
            })
            .ToListAsync();
    });

    [SkipOutput]
    public void Add() =>
        Location(Url.Page("/Invoices/Add"));

    [SkipOutput]
    public void Edit(string id) =>
        Location(Url.Page("/Invoices/Edit", new { id }));

    public void ClearSearch() =>
        SearchPhrase = null;

    public class Item
    {
        public string Id { get; set; }
        public DateTimeOffset IssueDate { get; set; }
        public string Number { get; set; }
        public string CustomerName { get; set; }
        public decimal ValueNet { get; set; }
        public string CurrencyCode { get; set; }
    }
}