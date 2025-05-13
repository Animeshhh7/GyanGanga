namespace GyanGanga.Web.Models.Classes
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
    }
}