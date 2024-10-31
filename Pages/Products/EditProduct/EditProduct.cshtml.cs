using System.ComponentModel.DataAnnotations;
using Hydro;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;
using HydroSales.Pages.Shared.Toasts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hydro.Scope;

namespace HydroSales.Pages.Products.EditProduct;

[ComponentAuthorize]
public class EditProduct(IDatabase database) : HydroComponent
{
    [Required]
    public string Id { get; set; }

    public string InitialName { get; set; }

    [Display(Name = "Name"), Required, MaxLength(50)]
    public string Name { get; set; }

    [Display(Name = "Code"), Required, MaxLength(50)]
    public string Code { get; set; }

    [Display(Name = "Price"), Required, Range(0, double.PositiveInfinity)]
    public decimal PriceNet { get; set; }

    [Display(Name = "Sales tax (%)"), Required, Range(0, 100)]
    public decimal SalesTax { get; set; }

    [Display(Name = "Currency"), Required]
    public string CurrencyCode { get; set; }

    public override async Task MountAsync()
    {
        var product = await database.Query<Product>(Id).SingleAsync();

        InitialName = product.Name;
        Name = product.Name;
        Code = product.Code;
        PriceNet = product.PriceNet;
        SalesTax = product.SalesTax;
        CurrencyCode = product.CurrencyCode;
    }

    public async Task Save()
    {
        if (!Validate())
        {
            return;
        }

        var product = await database.Query<Product>(Id).SingleAsync();

        product.Edit(
            name: Name,
            code: Code,
            priceNet: PriceNet,
            salesTax: SalesTax,
            currencyCode: CurrencyCode
        );

        await database.SaveChangesAsync();

        Back();
        Dispatch(new ShowMessage("Product has been updated"), Global);
    }

    public async Task Remove()
    {
        var product = await database.Query<Product>(Id).SingleAsync();
        product.Remove();
        await database.SaveChangesAsync();
        Back();
    }

    [SkipOutput]
    public void Back() =>
        Location(Url.Page("/Products/Index"));
}