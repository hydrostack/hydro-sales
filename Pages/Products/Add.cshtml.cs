using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HydroSales.Pages.Products;

[Authorize]
public class Add : PageModel;
