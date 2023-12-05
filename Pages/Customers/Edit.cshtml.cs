using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HydroSales.Pages.Customers;

[Authorize]
public class Edit : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Id { get; set; }
}