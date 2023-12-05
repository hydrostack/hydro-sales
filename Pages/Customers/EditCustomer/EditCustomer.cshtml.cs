using System.ComponentModel.DataAnnotations;
using Hydro;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using HydroSales.Pages.Shared.Toasts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hydro.Scope;

namespace HydroSales.Pages.Customers.EditCustomer;

[ComponentAuthorize]
public class EditCustomer(IDatabase database) : HydroComponent
{
    [Required]
    public string Id { get; set; }
    
    public string InitialName { get; set; }

    [Display(Name = "Name"), Required, MaxLength(50)]
    public string Name { get; set; }
    
    [Display(Name = "Tax ID"), MaxLength(20)]
    public string TaxId { get; set; }

    [Display(Name = "Currency"), Required]
    public string CurrencyCode { get; set; }

    [Display(Name = "Address"), MaxLength(255), Required]
    public string Address { get; set; }

    [Display(Name = "City"), MaxLength(255), Required]
    public string City { get; set; }

    [Display(Name = "Country"), Required]
    public string CountryCode { get; set; }
    
    [Display(Name = "Payment terms"), Required]
    public int PaymentTerms { get; set; }

    public override async Task MountAsync()
    {
        var customer = await database.Query<Customer>(Id).SingleAsync();

        InitialName = customer.Name;
        Name = customer.Name;
        TaxId = customer.TaxId;
        Address = customer.Address;
        City = customer.City;
        TaxId = customer.TaxId;
        CurrencyCode = customer.CurrencyCode;
        CountryCode = customer.CountryCode;
        PaymentTerms = customer.PaymentTerms;
    }

    public async Task Save()
    {
        if (!Validate())
        {
            return;
        }
        
        var customer = await database.Query<Customer>(Id).SingleAsync();

        customer.Edit(
            name: Name,
            taxId: TaxId,
            currencyCode: CurrencyCode,
            address: Address,
            city: City,
            countryCode: CountryCode,
            paymentTerms: PaymentTerms
        );

        await database.SaveChangesAsync();

        Back();
        Dispatch(new ShowMessage("Customer has been updated"), Global);
    }
    
    public async Task Remove()
    {
        var customer = await database.Query<Customer>(Id).SingleAsync();
        customer.Remove();
        await database.SaveChangesAsync();
        Back();
    }

    [SkipOutput]
    public void Back() =>
        Location(Url.Page("/Customers/Index"));
}
