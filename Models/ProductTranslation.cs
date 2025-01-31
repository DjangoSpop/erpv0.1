namespace erpv0._1.Models;

public partial class ProductTranslation
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Language { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Specifications { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
