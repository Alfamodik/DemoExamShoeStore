namespace ShoeStore.Core.Model;

public partial class PickUpPoint
{
    public int Id { get; set; }

    public string? PostIndex { get; set; }

    public string? Sity { get; set; }

    public string? Street { get; set; }

    public string? HomeNumber { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
