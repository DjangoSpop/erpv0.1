using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erpv0._1.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("InvoiceNoNavigation")]
        public int? InvoiceNo { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Amount { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public string Notes { get; set; }

        // DB relationships - using the exact naming convention from the entity configuration
        public virtual Invoice InvoiceNoNavigation { get; set; }

        // These fields are for backward compatibility with existing code
        public int? ShipmentId { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string ShipmentStatus { get; set; }
        public string ShipmentMethod { get; set; }
    }
}