using Hydro;
using HydroSales.Database;
using HydroSales.Domain;
using Microsoft.EntityFrameworkCore;

namespace HydroSales.Pages.Invoices.Common;

public class CustomerFormInfo(IDatabase database) : HydroComponent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public override async Task MountAsync()
    {
        if (Id == null)
        {
            return;
        }

        var customer = await database.Query<Customer>()
            .Where(c => c.Id == Id)
            .Select(c => new
            {
                c.Name,
                c.Address,
                c.City,
                c.CountryCode,
                c.PaymentTerms,
                c.CurrencyCode
            })
            .SingleAsync();

        Name = customer.Name;
        Address = customer.Address;
        City = customer.City;
        Country = CountryData.GetText(customer.CountryCode);
    }
}