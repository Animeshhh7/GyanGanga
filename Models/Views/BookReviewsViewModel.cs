namespace GyanGanga.Web.Models.Views
{
    public class BookReviewsViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
    }

    public class ReviewViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; }
    }
}