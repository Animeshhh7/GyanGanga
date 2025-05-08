namespace GyanGanga.Web.Models.ViewModels 
{ 
    public class BookDetailsViewModel 
    { 
        public int Id { get; set; } 
        public string? Title { get; set; } 
        public string? Author { get; set; } 
        public string? Genre { get; set; } 
        public decimal Price { get; set; } 
        public string? Format { get; set; } 
        public int Stock { get; set; } 
        public string? ISBN { get; set; } 
    } 
} 
