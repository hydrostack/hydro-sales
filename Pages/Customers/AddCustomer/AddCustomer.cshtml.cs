using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Hydro;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using HydroSales.Pages.Shared.Toasts;
using Microsoft.AspNetCore.Mvc;
using static Hydro.Scope;

namespace HydroSales.Pages.Customers.AddCustomer;

[ComponentAuthorize]
public class AddCustomer(IDatabase database) : HydroComponent
{
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

    public override void Mount()
    {
        PaymentTerms = 30;
    }

    public async Task Save()
    {
        if (!Validate())
        {
            return;
        }
        
        var customer = Customer.Create(
            user: await database.GetCurrentUser(),
            name: Name,
            taxId: TaxId,
            currencyCode: CurrencyCode,
            address: Address,
            city: City,
            countryCode: CountryCode,
            paymentTerms: PaymentTerms
        );

        await database.AddAsync(customer);
        await database.SaveChangesAsync();
        Back();
        Dispatch(new ShowMessage("Customer has been added"), Global);
    }

    [SkipOutput]
    public void Back() =>
        Location(Url.Page("/Customers/Index"));
}