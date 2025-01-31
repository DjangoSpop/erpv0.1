namespace erpv0._1.Models.ViewModels
{
    public class StockMovementViewModel
    {
        public int MovementId { get; set; }
        public string ProductName { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public string Reference { get; set; }
        public string WarehouseName { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
    }

    public class StockMovementListViewModel
    {
        public IEnumerable<StockMovementViewModel> Movements { get; set; }
        public int TotalMovements { get; set; }
        public int IncomingMovements { get; set; }
        public int OutgoingMovements { get; set; }
        public string SearchTerm { get; set; }
        public string MovementType { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
