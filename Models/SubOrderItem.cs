namespace erpv0._1.Models;

public partial class SubOrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual SubOrder? Order { get; set; }

    public virtual Product? Product { get; set; }
}
