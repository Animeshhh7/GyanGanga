namespace GyanGanga.Web.Models.Classes
{
    public class BookRating
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; } // 1 to 5 stars
        public DateTime RatedAt { get; set; }

        public BookRating(int bookId, string userId, int rating, DateTime ratedAt)
        {
            BookId = bookId;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Rating = rating;
            RatedAt = ratedAt;
        }
    }
}