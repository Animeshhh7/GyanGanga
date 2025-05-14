namespace GyanGanga.Web.Models.Classes
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!; // Required navigation property to Order
        public Book Book { get; set; } = null!;   // Required navigation property to Book
    }
}