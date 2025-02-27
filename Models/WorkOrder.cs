namespace erpv0._1.Models
{

    public partial class WorkOrder
    {
        public int WorkOrderId { get; set; }

        public int ProductId { get; set; }

        public int? Quantity { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Status { get; set; }
    }

}
