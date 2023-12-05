namespace HydroSales.Utils;

public static class EnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IEnumerable<Task<T>> tasks)
    {
        var result = new List<T>();
        
        foreach (var task in tasks)
        {
            result.Add(await task);
        }

        return result;
    }
}