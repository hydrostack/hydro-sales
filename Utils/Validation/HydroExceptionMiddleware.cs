using System.Net;
using Hydro;
using Microsoft.AspNetCore.Diagnostics;

namespace HydroSales.Utils.Validation;

public class HydroExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.IsHydro())
        {
            await next(httpContext);
            return;
        }

        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
        switch (contextFeature?.Error)
        {
            case DomainException domainException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { domainException.Message });
                return;

            default:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await httpContext.Response.WriteAsJsonAsync(new UnhandledHydroError(
                    Message: "There was a problem with this operation and it wasn't finished",
                    Data: null
                ));

                return;
        }
    }
}