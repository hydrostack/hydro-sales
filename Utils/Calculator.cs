using System.Collections;
using HydroSales.Domain;

namespace HydroSales.Utils;

public static class Calculator
{
    public static decimal Round(decimal amount) =>
        Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    public static decimal CalculateValue(decimal unitPrice, decimal quantity) =>
        Round(unitPrice * quantity);

    public static decimal CalculateTax(decimal net, decimal salesTax) =>
        Round(net * salesTax / 100M);

    public static decimal CalculateGross(decimal net, decimal salesTax) =>
        net + CalculateTax(net, salesTax);

    public static decimal ConvertCurrency(decimal amount, string sourceCurrencyCode, string destinationCurrencyCode) =>
        sourceCurrencyCode == null || destinationCurrencyCode == null || sourceCurrencyCode == destinationCurrencyCode
            ? amount
            : Round(amount * CurrencyData.GetValue(sourceCurrencyCode) / CurrencyData.GetValue(destinationCurrencyCode));

    public static IReadOnlyList<TaxGroup> GroupBySalesTax(IEnumerable<(decimal SalesTax, decimal ValueNet)> lines) =>
        lines
            .GroupBy(l => l.SalesTax)
            .Select(g => (
                SalesTax: g.Key,
                ValueNet: g.Sum(i => i.ValueNet)
            ))
            .Select(g => new TaxGroup(
                g.SalesTax,
                g.ValueNet,
                ValueTax: CalculateTax(g.ValueNet, g.SalesTax),
                ValueGross: CalculateGross(g.ValueNet, g.SalesTax)
            ))
            .ToList();
}

public record TaxGroup(decimal SalesTax, decimal ValueNet, decimal ValueTax, decimal ValueGross);