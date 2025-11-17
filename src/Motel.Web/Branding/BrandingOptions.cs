namespace Motel.Web.Branding;

/// <summary>
/// Branding configuration for reusability across different hospitality projects
/// </summary>
public class BrandingOptions
{
    public string SiteName { get; set; } = "نظام إدارة الموتيل";
    public string LogoPath { get; set; } = "/images/logo.png";
    public string PrimaryColor { get; set; } = "#2c3e50";
}
