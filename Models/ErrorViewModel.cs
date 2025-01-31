namespace erpv0._1.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public int StatusCode { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowStackTrace => !string.IsNullOrEmpty(StackTrace);
        public bool ShowInnerException => !string.IsNullOrEmpty(InnerException);
    }
}