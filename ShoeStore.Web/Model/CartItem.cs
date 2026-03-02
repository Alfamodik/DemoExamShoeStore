using System;
using System.Collections.Generic;

namespace ShoeStore.Web.Model;

public partial class CartItem
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ProductArticle { get; set; } = null!;

    public int Amount { get; set; }

    public virtual Product ProductArticleNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
