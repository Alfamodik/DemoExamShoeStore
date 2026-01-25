using System;
using System.Collections.Generic;

namespace ShoeStore.Model;

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

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ProductCategory? ProductCategory { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
