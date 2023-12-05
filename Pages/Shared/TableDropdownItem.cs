using System.Linq.Expressions;

namespace HydroSales.Pages.Shared;

public record TableDropdownItem(string Text, Expression<Action> Action);
