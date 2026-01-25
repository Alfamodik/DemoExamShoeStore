using System;
using System.Collections.Generic;

namespace ShoeStore.Model;

public partial class ProductCategory
{
    public int Id { get; set; }

    public string? ProductCategory1 { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
