namespace HydroSales.Domain;

public class CountryData
{
    public static readonly Dictionary<string, string> List = new()
    {
        { "PL", "Poland" },
        { "NO", "Norway" },
        { "IT", "Italy" },
        { "CZ", "Czech Republic" },
        { "FR", "France" },
        { "AU", "Australia" },
        { "EN", "England" },
        { "DE", "Germany" },
        { "SE", "Sweden" },
        { "CA", "Canada" }
    };
    
    public static string FindValue(string phrase) =>
        List
            .Where(c => string.Equals(c.Key, phrase, StringComparison.OrdinalIgnoreCase) || string.Equals(c.Value, phrase, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Key)
            .FirstOrDefault();
    
    public static string GetText(string value) =>
        List
            .Where(c => string.Equals(c.Key, value, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .FirstOrDefault();
}