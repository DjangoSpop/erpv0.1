namespace erpv0._1.Models;

public partial class WarehouseTransfer
{
    public int TransferId { get; set; }

    public int SourceWarehouseId { get; set; }

    public int DestWarehouseId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime RequestedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string ApprovedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public virtual Warehouse DestWarehouse { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse SourceWarehouse { get; set; } = null!;

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
