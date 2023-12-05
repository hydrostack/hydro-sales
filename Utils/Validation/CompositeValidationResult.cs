using System.ComponentModel.DataAnnotations;
using Hydro;

namespace HydroSales.Utils.Validation;

internal class CompositeValidationResult : ValidationResult, ICompositeValidationResult
{
    private readonly List<ValidationResult> _results = new();
    public IEnumerable<ValidationResult> Results => _results;
    public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
    public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
    protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

    public void AddResult(ValidationResult validationResult)
    {
        _results.Add(validationResult);
    }
}
