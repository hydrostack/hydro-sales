using Hydro;
using Microsoft.AspNetCore.Mvc;
using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Pages.Start;

namespace HydroSales.Pages.Shared.Profile;

public class ProfileDropdown(IAuthorizationService authorizationService, IDatabase database, IMembershipService membershipService) : HydroComponent
{
    public override async Task RenderAsync()
    {
        var user = await database.GetCurrentUser();
        ViewBag.IsAuthenticated = user != null;
    }

    public void SwitchTheme()
    {
        var theme = HttpContext.Request.Cookies.TryGetValue("theme", out var value) ? value : "emerald";
        var newTheme = theme == "emerald" ? "dark" : "emerald";
        HttpContext.Response.Cookies.Append("theme", newTheme, new CookieOptions { MaxAge = TimeSpan.FromDays(365) });
        Redirect(HttpContext.Request.Headers.Referer.ToString());
    }

    public async Task SignUp()
    {
        await membershipService.SignUp();
        Redirect(Url.Page("/Invoices/Index"));
    }

    public async Task Logout()
    {
        await authorizationService.SignOut();
        Redirect("/");
    }
}
