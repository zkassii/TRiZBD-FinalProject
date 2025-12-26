using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime DeliveryDate { get; set; }

    public int Code { get; set; }

    public bool IsFinished { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual User User { get; set; } = null!;
}
