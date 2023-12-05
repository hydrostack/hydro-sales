using Microsoft.AspNetCore.DataProtection;
using HydroSales.Pages.Shared;
using Newtonsoft.Json;

namespace HydroSales.Utils;

public interface ICookieStorageProvider
{
    CookieStorage Get();
    void Save(CookieStorage cookieStorage);
}

public class CookieStorageProvider(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectionProvider) : ICookieStorageProvider
{
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector(nameof(CookieStorageProvider));

    public CookieStorage Get()
    {
        try
        {
            return httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue("storage", out var storage)
                ? JsonConvert.DeserializeObject<CookieStorage>(_protector.Unprotect(storage))
                : null;
        }
        catch
        {
            return null;
        }
    }

    public void Save(CookieStorage cookieStorage)
    {
        try
        {
            var response = httpContextAccessor.HttpContext!.Response;
            if (cookieStorage != null)
            {
                var serializeObject = _protector.Protect(JsonConvert.SerializeObject(cookieStorage));
                response.Cookies.Append("storage", serializeObject, new CookieOptions { MaxAge = TimeSpan.FromDays(365) });
            }
            else
            {
                response.Cookies.Delete("storage");
            }
        }
        catch
        {
            // ignored
        }
    }
}