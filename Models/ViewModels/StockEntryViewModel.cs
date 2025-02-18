using erpv0._1.Models;
using System.Collections.Generic;

namespace erpv0._1.Models.ViewModels
{
        public class StockEntryIndexViewModel
        {
            public List<StockEntry> StockEntries { get; set; } = new();
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public int PageSize { get; set; }
            public string? SearchTerm { get; set; }
            public int? WarehouseId { get; set; }
            public int? ProductId { get; set; }
        }
    }


