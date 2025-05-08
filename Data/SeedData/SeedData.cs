using GyanGanga.Web.Models.Entities;

namespace GyanGanga.Web.Data
{
    public static class SeedData
    {
        public static void Initialize(GyanGangaDbContext context)
        {
            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    new Book
                    {
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        Genre = "Fiction",
                        Price = 9.99m,
                        Format = "Paperback",
                        Stock = 50,
                        ISBN = "978-0743273565"
                    },
                    new Book
                    {
                        Title = "1984",
                        Author = "George Orwell",
                        Genre = "Dystopian",
                        Price = 12.50m,
                        Format = "Hardcover",
                        Stock = 30,
                        ISBN = "978-0451524935"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}