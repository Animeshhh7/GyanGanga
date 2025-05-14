namespace GyanGanga.Web.Models.Classes
{
    public class ShowBook
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public string BookGenre { get; set; } = string.Empty;
        public decimal BookPrice { get; set; }
        public string BookFormat { get; set; } = string.Empty;
        public int BookStock { get; set; }
        public string BookIsbn { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int Quantity { get; set; }
        public bool IsBookmarked { get; set; }
        public string CoverImagePath { get; set; } = string.Empty;
        public int ReviewCount { get; set; } // Added to store the number of reviews
    }
}