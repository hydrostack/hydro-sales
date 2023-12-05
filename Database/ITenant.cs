using HydroSales.Domain;

namespace HydroSales.Database;

public interface ITenant
{
    public User User { get; }
}