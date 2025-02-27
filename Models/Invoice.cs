using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erpv0._1.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            Payments = new HashSet<Payment>();
        }

        [Key]
        public int InvoiceNo { get; set; }

        public int? SupplierId { get; set; }

        public DateOnly? InvoiceDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? Notes { get; set; }

        public int? OrderId { get; set; }

        public virtual SubOrder? Order { get; set; }

        // Collection navigation property with the correct naming per the entity configuration
        public virtual ICollection<Payment> Payments { get; set; }

        public virtual Supplier? Supplier { get; set; }
    }
}