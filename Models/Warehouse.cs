namespace erpv0._1.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<StockEntry> StockEntries { get; set; } = new List<StockEntry>();

    public virtual ICollection<WarehouseTransfer> WarehouseTransferDestWarehouses { get; set; } = new List<WarehouseTransfer>();

    public virtual ICollection<WarehouseTransfer> WarehouseTransferSourceWarehouses { get; set; } = new List<WarehouseTransfer>();
}
