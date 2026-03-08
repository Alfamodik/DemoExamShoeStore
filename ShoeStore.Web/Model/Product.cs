using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ShoeStore.Web.Model;

public partial class Product
{
    public string Article { get; set; } = null!;

    public string? Product1 { get; set; }

    public string? Unit { get; set; }

    public decimal? Cost { get; set; }

    public int? SupplierId { get; set; }

    public int? ManufacturerId { get; set; }

    public int? ProductCategoryId { get; set; }

    public decimal? Discount { get; set; }

    public int? AmountInStorage { get; set; }

    public string? Description { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ProductCategory? ProductCategory { get; set; }

    public virtual Supplier? Supplier { get; set; }

    public string CategoruAndName => $"{ProductCategory?.ProductCategory1} | {Product1}";

    public string? CostFormated => Cost.ToString();

    public decimal CostWithDiscountValue => (decimal)(Cost!.Value - Cost* Discount / 100)!;
    public string CostWithDiscount
    {
        get
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("ru-RU");
            decimal cost = (decimal)(Cost!.Value - Cost * Discount / 100)!;
            return cost.ToString("F2", culture);
        }
    }

    public string DisplayName => $"{Article} {Product1} {Cost}";
}
