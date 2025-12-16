namespace FurnitureShop.Models
{
    public class Furniture
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? StorageRow { get; set; }
        public string? StorageShelf { get; set; }
        public string? ImagePath { get; set; }
        public string? ImagePath2 { get; set; }
        public string? ImagePath3 { get; set; }
        public string? SerialNumber { get; set; }
        public int? CategoryId { get; set; }

        // Navigation property
        public Category? Category { get; set; }
    }
}
