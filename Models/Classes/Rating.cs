namespace GyanGanga.Web.Models.Classes
{
    public class Rating
    {
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Value { get; set; } // Rating value (1 to 5)

        // Navigation properties
        public Book Book { get; set; } = null!;
    }
}