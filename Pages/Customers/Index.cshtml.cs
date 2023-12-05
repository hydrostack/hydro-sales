using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HydroSales.Pages.Customers;

[Authorize]
public class Index : PageModel;
