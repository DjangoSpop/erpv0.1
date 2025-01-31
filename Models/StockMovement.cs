namespace erpv0._1.Models;

public partial class StockMovement
{
    public int MovementId { get; set; }

    public int ProductId { get; set; }

    public int StockEntryId { get; set; }

    public int? TransferId { get; set; }

    public string MovementType { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime MovementDate { get; set; }

    public string Reference { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual StockEntry StockEntry { get; set; } = null!;

    public virtual WarehouseTransfer? Transfer { get; set; }
}
