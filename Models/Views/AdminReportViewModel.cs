namespace GyanGanga.Web.Models.Views
{
    public class AdminReportViewModel
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public List<TopSellingBook> TopSellingBooks { get; set; }
        public List<UserActivity> UserActivity { get; set; }

        public AdminReportViewModel()
        {
            TopSellingBooks = new List<TopSellingBook>();
            UserActivity = new List<UserActivity>();
        }
    }

    public class TopSellingBook
    {
        public string? BookTitle { get; set; }
        public int BookId { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class UserActivity
    {
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public int OrderCount { get; set; }
    }
}