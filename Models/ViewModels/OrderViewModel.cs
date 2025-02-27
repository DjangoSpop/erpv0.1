using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        [Display(Name = "العميل")]
        public int? CustomerId { get; set; }

        [Display(Name = "اسم العميل")]
        public string CustomerName { get; set; }

        [Display(Name = "حالة الطلب")]
        [Required(ErrorMessage = "حالة الطلب مطلوبة")]
        [Range(1, 5, ErrorMessage = "حالة الطلب يجب أن تكون بين 1 و 5")]
        public byte OrderStatus { get; set; }

        [Display(Name = "حالة الطلب")]
        public string OrderStatusName => GetOrderStatusName(OrderStatus);

        [Display(Name = "تاريخ الطلب")]
        [Required(ErrorMessage = "تاريخ الطلب مطلوب")]
        [DataType(DataType.Date)]
        public DateOnly OrderDate { get; set; }

        [Display(Name = "تاريخ التسليم المطلوب")]
        [Required(ErrorMessage = "تاريخ التسليم المطلوب مطلوب")]
        [DataType(DataType.Date)]
        public DateOnly RequiredDate { get; set; }

        [Display(Name = "تاريخ الشحن")]
        [DataType(DataType.Date)]
        public DateOnly? ShippedDate { get; set; }

        [Display(Name = "المتجر")]
        [Required(ErrorMessage = "المتجر مطلوب")]
        public int StoreId { get; set; }

        [Display(Name = "اسم المتجر")]
        public string StoreName { get; set; }

        [Display(Name = "الموظف")]
        [Required(ErrorMessage = "الموظف مطلوب")]
        public int StaffId { get; set; }

        [Display(Name = "اسم الموظف")]
        public string StaffName { get; set; }

        [Display(Name = "عدد العناصر")]
        public int ItemCount { get; set; }

        [Display(Name = "المبلغ الإجمالي")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        public static string GetOrderStatusName(byte status)
        {
            return status switch
            {
                1 => "قيد الانتظار",
                2 => "معالجة",
                3 => "تم الشحن",
                4 => "تم التسليم",
                5 => "ملغى",
                _ => "غير معروف"
            };
        }

        public static List<KeyValuePair<byte, string>> GetOrderStatuses()
        {
            return new List<KeyValuePair<byte, string>>
            {
                new KeyValuePair<byte, string>(1, "قيد الانتظار"),
                new KeyValuePair<byte, string>(2, "معالجة"),
                new KeyValuePair<byte, string>(3, "تم الشحن"),
                new KeyValuePair<byte, string>(4, "تم التسليم"),
                new KeyValuePair<byte, string>(5, "ملغى")
            };
        }
    }
}
