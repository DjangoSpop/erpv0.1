using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels.ProductViewModel
{
    public class UpdateProductViewModel : CreateProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "تم الإنشاء بواسطة")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "الكمية الحالية")]
        public int CurrentStock { get; set; }

        [Display(Name = "القيمة الإجمالية")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalValue => Price * CurrentStock;
    }
}
