using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Hydro;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using HydroSales.Pages.Shared.Toasts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hydro.Scope;

namespace HydroSales.Pages.Products.AddProduct;

[ComponentAuthorize]
public class AddProduct(IDatabase database) : HydroComponent
{
    [Display(Name = "Name"), Required, MaxLength(50)]
    public string Name { get; set; }
    
    [Display(Name = "Code"), Required, MaxLength(50)]
    public string Code { get; set; }

    [Display(Name = "Price"), Required, Range(0, double.PositiveInfinity, MinimumIsExclusive = true)]
    public decimal PriceNet { get; set; }

    [Display(Name = "Sales tax (VAT)"), Required, Range(0, 100, MaximumIsExclusive = true)]
    public decimal SalesTax { get; set; }

    [Display(Name = "Currency"), Required]
    public string CurrencyCode { get; set; }

    public override async Task MountAsync()
    {
        var userSettings = await database.Query<UserSettings>().SingleAsync();
        
        CurrencyCode = userSettings.DefaultCurrencyCode;
    }
    
    public async Task Save()
    {
        if (!Validate())
        {
            return;
        }
        
        var product = Product.Create(
            user: await database.GetCurrentUser(),
            name: Name, 
            code: Code,
            priceNet: PriceNet,
            salesTax: SalesTax,
            currencyCode: CurrencyCode
        );

        await database.AddAsync(product);
        await database.SaveChangesAsync();
        Back();
        Dispatch(new ShowMessage("Product has been added"), Global);
    }

    [SkipOutput]
    public void Back() =>
        Location(Url.Page("/Products/Index"));

    public class Validator : AbstractValidator<AddProduct>
    {
        public Validator()
        {
            RuleFor(c => c.CurrencyCode).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}