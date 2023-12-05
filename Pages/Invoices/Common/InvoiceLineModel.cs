using System.ComponentModel.DataAnnotations;

namespace HydroSales.Pages.Invoices.Common;

public class InvoiceLineModel
{
    public string Id { get; set; }
    
    [Display(Name = "Product"), Required]
    public string ProductId { get; set; }
    
    [Display(Name = "Currency"), Required]
    public string CurrencyCode { get; set; }

    [Display(Name = "Tax (% VAT)"), Required, Range(0, 100, MaximumIsExclusive = true)]
    public decimal SalesTax { get; set; }
    
    [Display(Name = "Unit price"), Required, Range(0, double.PositiveInfinity, MinimumIsExclusive = true)]
    public decimal UnitPriceNet { get; set; }

    [Display(Name = "Qty"), Required]
    public decimal Quantity { get; set; }
    
    [Display(Name = "Unit"), Required, MaxLength(20)]
    public string Unit { get; set; }
    
    [Display(Name = "Net")]
    public decimal BaseValueNet { get; set; }

    [Display(Name = "Net")]
    public decimal ValueNet { get; set; }
    
    [Display(Name = "Gross")]
    public decimal ValueGross { get; set; }
}