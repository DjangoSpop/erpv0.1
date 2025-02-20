using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class SupplierViewModel
    {
        public int SupplierId { get; set; }

        [Display(Name = "اسم المورد")]
        [Required(ErrorMessage = "اسم المورد مطلوب")]
        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم 100 حرف")]
        public string Name { get; set; }

        [Display(Name = "اسم جهة الاتصال")]
        [Required(ErrorMessage = "اسم جهة الاتصال مطلوب")]
        public string ContactName { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string ContactEmail { get; set; }

        [Display(Name = "رقم الهاتف")]
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
        public string ContactPhone { get; set; }

        [Display(Name = "المدينة")]
        [Required(ErrorMessage = "المدينة مطلوبة")]
        public string City { get; set; }

        [Display(Name = "العنوان")]
        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }

        [Display(Name = "عدد الفواتير")]
        public int InvoiceCount { get; set; }
    }
}
