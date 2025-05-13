namespace GyanGanga.Web.Models.Views
{
    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}