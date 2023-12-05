using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using HydroSales.Domain;

namespace HydroSales.Authorization;

public interface IAuthorizationService
{
    string GetCurrentUserName();
    string GetCurrentUserEmail();
    string GetCurrentUserId();
    Task SignIn(string userId, string userName);
    Task SignOut();
    bool IsAuthenticated();
    Task SignIn(User user);
}

public class WebAuthorizationService(IHttpContextAccessor httpContextAccessor) : IAuthorizationService
{
    public bool IsAuthenticated() =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public string GetCurrentUserName() =>
        httpContextAccessor.HttpContext?.User.Claims
            .Where(claim => claim.Type == ClaimTypes.Name)
            .Select(claim => claim.Value)
            .SingleOrDefault();

    public string GetCurrentUserId() =>
        httpContextAccessor.HttpContext?.User.Claims
            .Where(claim => claim.Type == ClaimTypes.NameIdentifier)
            .Select(claim => claim.Value)
            .SingleOrDefault();

    public string GetCurrentUserEmail() =>
        httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == ClaimTypes.Email)
            .Select(claim => claim.Value)
            .SingleOrDefault();

    public Task SignIn(User user) =>
        SignIn(user.Id, user.Id);

    public async Task SignIn(string userId, string userName)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new(claimsIdentity),
            new() { IsPersistent = true }
        );
    }

    public Task SignOut() =>
        httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}