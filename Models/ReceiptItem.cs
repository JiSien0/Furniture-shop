namespace FurnitureShop.Models
{
    public class ReceiptItem
    {
        public int Id { get; set; }
        public int ReceiptId { get; set; }
        public int FurnitureId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
