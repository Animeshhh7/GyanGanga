using GyanGanga.Web.Models.Classes; 
 
namespace GyanGanga.Web.Data 
{ 
    public class StartBooks 
    { 
        // Add some books to the database if it's empty 
        public static void AddBooks(MyDB db) 
        { 
            if (!db.BookList.Any()) 
            { 
                db.BookList.Add(new Book 
                { 
                    BookTitle = "The Great Gatsby", 
                    BookAuthor = "F. Scott Fitzgerald", 
                    BookGenre = "Fiction", 
                    BookPrice = 10.50m, 
                    BookFormat = "Paperback", 
                    BookStock = 40, 
                    BookIsbn = "978-0743273565" 
                }); 
                db.BookList.Add(new Book 
                { 
                    BookTitle = "Nineteen Eighty-Four", 
                    BookAuthor = "George Orwell", 
                    BookGenre = "Dystopian", 
                    BookPrice = 13.00m, 
                    BookFormat = "Hardcover", 
                    BookStock = 20, 
                    BookIsbn = "978-0451524935" 
                }); 
                db.SaveChanges(); 
            } 
        } 
    } 
} 
