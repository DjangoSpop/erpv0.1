using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class SettingsViewModel
    {
        public int SettingId { get; set; }

        [Required(ErrorMessage = "اسم النظام مطلوب")]
        [Display(Name = "اسم النظام")]
        public string SystemName { get; set; } = string.Empty;

        [Display(Name = "شعار النظام")]
        public string? LogoPath { get; set; }

        [Required(ErrorMessage = "اللغة الافتراضية مطلوبة")]
        [Display(Name = "اللغة الافتراضية")]
        public string DefaultLanguage { get; set; } = "ar";

        [Required(ErrorMessage = "المنطقة الزمنية مطلوبة")]
        [Display(Name = "المنطقة الزمنية")]
        public string TimeZone { get; set; } = "Arab Standard Time";

        [Required(ErrorMessage = "تنسيق التاريخ مطلوب")]
        [Display(Name = "تنسيق التاريخ")]
        public string DateFormat { get; set; } = "dd/MM/yyyy";

        [Display(Name = "تفعيل الإشعارات")]
        public bool EnableNotifications { get; set; } = true;

        [Display(Name = "حد المخزون المنخفض")]
        [Range(1, 1000, ErrorMessage = "حد المخزون المنخفض يجب أن يكون بين 1 و 1000")]
        public int LowStockThreshold { get; set; } = 10;

        [Display(Name = "تفعيل سجل المراجعة")]
        public bool EnableAuditLog { get; set; } = true;

        [Required(ErrorMessage = "العملة مطلوبة")]
        [Display(Name = "العملة")]
        public string Currency { get; set; } = "SAR";

        [Required(ErrorMessage = "تنسيق العملة مطلوب")]
        [Display(Name = "تنسيق العملة")]
        public string CurrencyFormat { get; set; } = "#,##0.00";

        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [Display(Name = "البريد الإلكتروني للنظام")]
        public string? SystemEmail { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "عنوان الشركة")]
        public string? CompanyAddress { get; set; }

        [Range(0, 100, ErrorMessage = "نسبة الضريبة يجب أن تكون بين 0 و 100")]
        [Display(Name = "الضريبة الافتراضية %")]
        public decimal DefaultTaxRate { get; set; } = 15;

        // Dropdown lists for UI selections
        public List<SelectListItem> AvailableLanguages { get; set; } = new();
        public List<SelectListItem> AvailableTimeZones { get; set; } = new();
        public List<SelectListItem> AvailableDateFormats { get; set; } = new();
        public List<SelectListItem> AvailableCurrencies { get; set; } = new();
    }
}
