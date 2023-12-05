using Hydro;
using HydroSales.Database;

namespace HydroSales.Authorization;

public sealed class ComponentAuthorizeAttribute : Attribute, IHydroAuthorizationFilter
{
    public async Task<bool> AuthorizeAsync(HttpContext httpContext, object component)
    {
        var database = httpContext.RequestServices.GetRequiredService<IDatabase>();
        return await database.GetCurrentUser() != null;
    }
}
