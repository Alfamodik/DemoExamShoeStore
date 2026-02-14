namespace ShoeStore.Core.Model;

public partial class Order
{
    public int Id { get; set; }

    public string? ProductArticle { get; set; }

    public int? Amount { get; set; }

    public DateOnly? DateOrder { get; set; }

    public DateOnly? DateDelivery { get; set; }

    public int? PickUpPointId { get; set; }

    public int? UserId { get; set; }

    public string? Code { get; set; }

    public string? Status { get; set; }

    public virtual PickUpPoint? PickUpPoint { get; set; }

    public virtual Product? ProductArticleNavigation { get; set; }

    public virtual User? User { get; set; }
}
