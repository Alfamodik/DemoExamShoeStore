using System;
using System.Collections.Generic;

namespace ShoeStore.Web.Model;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string ProductArticle { get; set; } = null!;

    public int Amount { get; set; }

    public decimal Price { get; set; }

    public decimal Discount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product ProductArticleNavigation { get; set; } = null!;
}
