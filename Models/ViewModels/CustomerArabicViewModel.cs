namespace erpv0._1.Models.ViewModels
{
    public class CustomerArabicViewModel
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
