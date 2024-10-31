using Hydro;
using HydroSales.Database;
using Microsoft.AspNetCore.Mvc;

namespace HydroSales.Pages.Start.Components;

public class RegisterPanel(IMembershipService membershipService, IDatabase database) : HydroComponent
{
    public override async Task RenderAsync()
    {
        var user = await database.GetCurrentUser();
        ViewBag.IsAuthenticated = user != null;
    }

    public async Task Register()
    {
        await membershipService.SignUp();
        Redirect(Url.Page("/Invoices/Index"));
    }
}