

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace erpv0._1.Models

{
    [Table("SystemSettings")]
    public class SystemSettings
    {
        [Key]
        public int SettingId { get; set; }

        [Required]
        [Display(Name = "اسم النظام")]
        public string SystemName { get; set; } = "نظام إدارة المستودعات";

        [Required]
        [Display(Name = "شعار النظام")]
        public string? LogoPath { get; set; }

        [Required]
        [Display(Name = "اللغة الافتراضية")]
        public string DefaultLanguage { get; set; } = "ar";

        [Required]
        [Display(Name = "المنطقة الزمنية")]
        public string TimeZone { get; set; } = "Arab Standard Time";

        [Required]
        [Display(Name = "تنسيق التاريخ")]
        public string DateFormat { get; set; } = "dd/MM/yyyy";

        [Display(Name = "تفعيل الإشعارات")]
        public bool EnableNotifications { get; set; } = true;

        [Display(Name = "حد المخزون المنخفض")]
        public int LowStockThreshold { get; set; } = 10;

        [Display(Name = "تفعيل سجل المراجعة")]
        public bool EnableAuditLog { get; set; } = true;

        [Display(Name = "عملة النظام")]
        public string Currency { get; set; } = "SAR";

        [Display(Name = "تنسيق العملة")]
        public string CurrencyFormat { get; set; } = "#,##0.00";

        [Display(Name = "البريد الإلكتروني للنظام")]
        [EmailAddress]
        public string? SystemEmail { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "عنوان الشركة")]
        public string? CompanyAddress { get; set; }

        [Display(Name = "الضريبة الافتراضية %")]
        [Range(0, 100)]
        public decimal DefaultTaxRate { get; set; } = 15;

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
