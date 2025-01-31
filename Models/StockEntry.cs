using System;
using System.Collections.Generic;

namespace erpv0._1.Models;

public partial class StockEntry
{
    public int EntryId { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public string BatchNumber { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal MaxDiscount { get; set; }

    public DateTime ReceiptDate { get; set; }

    public string SupplierInvoice { get; set; } = null!;

    public int? ProductVariantVariantId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;


    public virtual Product Product { get; set; } = null!;

    public virtual ProductVariant? ProductVariantVariant { get; set; }

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    public virtual Warehouse Warehouse { get; set; } = null!;
}
