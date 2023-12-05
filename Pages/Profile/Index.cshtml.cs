using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace HydroSales.Pages.Profile;

[Authorize]
public class Index : PageModel;
