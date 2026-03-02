using System;
using System.Collections.Generic;

namespace ShoeStore.Web.Model;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime DateOrder { get; set; }

    public DateTime? DateDelivery { get; set; }

    public int PickUpPointId { get; set; }

    public string Code { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual PickUpPoint PickUpPoint { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
