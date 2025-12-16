using System;

namespace FurnitureShop.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public bool Delivery { get; set; }
    }
}
