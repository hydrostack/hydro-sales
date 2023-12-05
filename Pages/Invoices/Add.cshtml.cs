using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HydroSales.Pages.Invoices;

[Authorize]
public class Add : PageModel;
