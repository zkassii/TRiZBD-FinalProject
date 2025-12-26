namespace ShoeStoreLibrary.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Article { get; set; } = null!;

    public int CategoryId { get; set; }

    public string Unit { get; set; } = null!;

    public int Price { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturerId { get; set; }

    public byte? Discount { get; set; }

    public int Quantity { get; set; }

    public string? Gender { get; set; }

    public string? Description { get; set; }

    public string? Color { get; set; }

    public byte? Size { get; set; }

    public string? Image { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Supplier Supplier { get; set; } = null!;
}
