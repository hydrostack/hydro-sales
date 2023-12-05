namespace HydroSales.Utils;

public static class IdProvider
{
    public static string NewId() => Guid.NewGuid().ToString("N");
}