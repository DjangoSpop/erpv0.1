namespace erpv0._1.Models
{
    public partial class ProductionSchedule
    {
        public int? Schedule { get; set; }

        public int WorkOrderId { get; set; }

        public int? MachineId { get; set; }

        public int? EmployeeId { get; set; }

        public int? TimeSlots { get; set; }

        public virtual Machine? Machine { get; set; }

        public virtual WorkOrder WorkOrder { get; set; } = null!;
    }
}
