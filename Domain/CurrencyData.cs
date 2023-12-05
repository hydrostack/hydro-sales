namespace HydroSales.Domain;

public class CurrencyData
{
    public static readonly Dictionary<string, decimal> List = new()
    {
        { "PLN", 0.24840961M },
        { "EUR", 1.0769337M },
        { "GBP", 1.2869337M },
        { "AUD", 1.1769337M },
        { "USD", 1 },
        { "NOK", 0.09161058M },
    };
    
    public static string FindKey(string phrase) =>
        List
            .Where(c => string.Equals(c.Key, phrase, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Key)
            .FirstOrDefault();

    public static decimal GetValue(string key) =>
        List
            .Where(c => string.Equals(c.Key, key, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .FirstOrDefault();

    public static string GetText(string value) =>
        List
            .Where(c => string.Equals(c.Key, value, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Key)
            .FirstOrDefault();
}