using GyanGanga.Web.Data;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace GyanGanga.Web.Services
{
    public class BookHelper : IBookHelper
    {
        private readonly MyDB _db;

        public BookHelper(MyDB db)
        {
            _db = db;
        }

        public async Task<List<ShowBook>> GetAllBooks()
        {
            var books = await _db.BookList.ToListAsync() ?? new List<Book>();
            var result = books.Select(b => new ShowBook
            {
                BookId = b.BookId,
                BookTitle = b.BookTitle ?? "",
                BookAuthor = b.BookAuthor ?? "",
                BookGenre = b.BookGenre ?? "",
                BookPrice = b.BookPrice,
                BookFormat = b.BookFormat ?? "",
                BookStock = b.BookStock,
                BookIsbn = b.BookIsbn ?? "",
                Rating = b.Rating ?? 4.5m,
                Quantity = 0
            }).ToList();
            return result;
        }

        public async Task<Book> GetBookEntityById(int id)
        {
            return await _db.BookList.FindAsync(id);
        }

        public async Task AddBook(Book book)
        {
            // Generate a new BookId (simple increment for demo purposes; in a real app, use an auto-incrementing ID)
            var maxId = await _db.BookList.MaxAsync(b => (int?)b.BookId) ?? 0;
            book.BookId = maxId + 1;
            _db.BookList.Add(book);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateBook(Book book)
        {
            var existingBook = await _db.BookList.FindAsync(book.BookId);
            if (existingBook != null)
            {
                existingBook.BookTitle = book.BookTitle;
                existingBook.BookAuthor = book.BookAuthor;
                existingBook.BookGenre = book.BookGenre;
                existingBook.BookPrice = book.BookPrice;
                existingBook.BookFormat = book.BookFormat;
                existingBook.BookStock = book.BookStock;
                existingBook.BookIsbn = book.BookIsbn;
                existingBook.Rating = book.Rating;
                _db.BookList.Update(existingBook);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteBook(int id)
        {
            var book = await _db.BookList.FindAsync(id);
            if (book != null)
            {
                // Remove associated cart items and bookmarks
                var cartItems = await _db.CartBooks.Where(c => c.BookId == id).ToListAsync();
                var bookmarks = await _db.Bookmarks.Where(b => b.BookId == id).ToListAsync();
                if (cartItems.Any()) _db.CartBooks.RemoveRange(cartItems);
                if (bookmarks.Any()) _db.Bookmarks.RemoveRange(bookmarks);
                _db.BookList.Remove(book);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<ShowBookDetails?> GetBookById(int id)
        {
            var book = await _db.BookList.FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return null;
            }
            var result = new ShowBookDetails
            {
                BookId = book.BookId,
                BookTitle = book.BookTitle,
                BookAuthor = book.BookAuthor,
                BookGenre = book.BookGenre,
                BookPrice = book.BookPrice,
                BookFormat = book.BookFormat,
                BookStock = book.BookStock,
                BookIsbn = book.BookIsbn
            };
            return result;
        }

        public async Task SaveBook(int bookId, string userId)
        {
            bool doesExist = await _db.Bookmarks.AnyAsync(s => s.UserId == userId && s.BookId == bookId);
            if (!doesExist)
            {
                var bookmark = new Bookmarks
                {
                    BookId = bookId,
                    UserId = userId
                };
                _db.Bookmarks.Add(bookmark);
                await _db.SaveChangesAsync();
            }
        }

        public async Task Unbookmark(int bookId, string userId)
        {
            var bookmark = await _db.Bookmarks.FirstOrDefaultAsync(s => s.UserId == userId && s.BookId == bookId);
            if (bookmark != null)
            {
                _db.Bookmarks.Remove(bookmark);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsBookmarked(int bookId, string userId)
        {
            return await _db.Bookmarks.AnyAsync(s => s.UserId == userId && s.BookId == bookId);
        }

        public async Task AddToCart(int bookId, string userId, int quantity)
        {
            var cartItem = await _db.CartBooks.FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                _db.CartBooks.Update(cartItem);
            }
            else
            {
                var newItem = new CartBook
                {
                    BookId = bookId,
                    UserId = userId,
                    Quantity = quantity
                };
                _db.CartBooks.Add(newItem);
            }
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCartItemQuantity(int bookId, string userId, int quantity)
        {
            var cartItem = await _db.CartBooks.FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _db.CartBooks.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    _db.CartBooks.Update(cartItem);
                }
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCart(int bookId, string userId)
        {
            var cartItem = await _db.CartBooks.FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
            if (cartItem != null)
            {
                _db.CartBooks.Remove(cartItem);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<ShowBook>> GetSavedBooks(string userId)
        {
            var bookmarksQuery = _db.Bookmarks.Where(s => s.UserId == userId);
            var bookmarks = await bookmarksQuery.Join(_db.BookList,
                s => s.BookId,
                b => b.BookId,
                (s, b) => new { Bookmark = s, Book = b }).ToListAsync();
            var result = bookmarks != null ? bookmarks.Select(x => new ShowBook
            {
                BookId = x.Book.BookId,
                BookTitle = x.Book.BookTitle ?? "",
                BookAuthor = x.Book.BookAuthor ?? "",
                BookGenre = x.Book.BookGenre ?? "",
                BookPrice = x.Book.BookPrice,
                BookFormat = x.Book.BookFormat ?? "",
                BookStock = x.Book.BookStock,
                BookIsbn = x.Book.BookIsbn ?? "",
                Rating = x.Book.Rating ?? 4.5m,
                Quantity = 0,
                IsBookmarked = true
            }).ToList() : new List<ShowBook>();
            return result;
        }

        public async Task<List<ShowBook>> GetCartBooks(string userId)
        {
            var cartBooksQuery = _db.CartBooks.Where(c => c.UserId == userId);
            var cartBooks = await cartBooksQuery.Join(_db.BookList,
                c => c.BookId,
                b => b.BookId,
                (c, b) => new { CartBook = c, Book = b }).ToListAsync();
            var result = cartBooks != null ? cartBooks.Select(x => new ShowBook
            {
                BookId = x.Book.BookId,
                BookTitle = x.Book.BookTitle ?? "",
                BookAuthor = x.Book.BookAuthor ?? "",
                BookGenre = x.Book.BookGenre ?? "",
                BookPrice = x.Book.BookPrice,
                BookFormat = x.Book.BookFormat ?? "",
                BookStock = x.Book.BookStock,
                BookIsbn = x.Book.BookIsbn ?? "",
                Rating = x.Book.Rating ?? 4.5m,
                Quantity = x.CartBook.Quantity
            }).ToList() : new List<ShowBook>();
            return result;
        }

        // Admin-specific methods
        public async Task<List<(CartBook CartItem, Book Book)>> GetAllCartItems()
        {
            var cartItemsQuery = _db.CartBooks;
            var cartItems = await cartItemsQuery.Join(_db.BookList,
                c => c.BookId,
                b => b.BookId,
                (c, b) => new { CartItem = c, Book = b }).ToListAsync();
            return cartItems != null ? cartItems.Select(x => (x.CartItem, x.Book)).ToList() : new List<(CartBook, Book)>();
        }

        public async Task RemoveUserCartAndBookmarks(string userId)
        {
            var cartItems = await _db.CartBooks.Where(c => c.UserId == userId).ToListAsync();
            var bookmarks = await _db.Bookmarks.Where(b => b.UserId == userId).ToListAsync();
            if (cartItems.Any()) _db.CartBooks.RemoveRange(cartItems);
            if (bookmarks.Any()) _db.Bookmarks.RemoveRange(bookmarks);
            await _db.SaveChangesAsync();
        }
    }
}