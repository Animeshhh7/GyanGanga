using System;

namespace GyanGanga.Web.Models.Classes
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Review text (up to 50 characters)
        public DateTime PostedDate { get; set; }

        // Navigation properties
        public Book Book { get; set; } = null!;
    }
}