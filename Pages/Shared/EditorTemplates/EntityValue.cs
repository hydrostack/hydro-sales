namespace HydroSales.Pages.Shared.EditorTemplates;

public class EntityValue
{
    public EntityValue()
    {
    }

    public EntityValue(string id, string value)
    {
        Id = id;
        Value = value;
    }

    public string Id { get; set; }
    public string Value { get; set; }
};
