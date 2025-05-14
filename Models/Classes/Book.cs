namespace GyanGanga.Web.Models.Classes
{
    public class Book
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookAuthor { get; set; }
        public string? BookGenre { get; set; }
        public decimal BookPrice { get; set; }
        public string? BookFormat { get; set; }
        public int BookStock { get; set; }
        public string? BookIsbn { get; set; }
        public decimal? Rating { get; set; }
        public string? CoverImagePath { get; set; } // Added for image upload
    }
}