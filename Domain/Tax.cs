namespace HydroSales.Domain;

public class Tax
{
    public string Id { get; set; }
    public bool IsRemoved { get; set; }
    public string Name { get; set; }
    public decimal Value { get; set; }
}