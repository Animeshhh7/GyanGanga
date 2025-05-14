using GyanGanga.Web.Models.Classes;

namespace GyanGanga.Web.Models.Views
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }
        public bool CanRate { get; set; }
        public int? UserRating { get; set; }

        public BookDetailsViewModel(Book book)
        {
            Book = book ?? throw new ArgumentNullException(nameof(book));
        }
    }
}