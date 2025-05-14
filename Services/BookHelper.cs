using GyanGanga.Web.Data;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Models.Views;
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
                Quantity = 0,
                CoverImagePath = b.CoverImagePath ?? "" // Add fallback for null
            }).ToList();
            return result;
        }

        public async Task<Book?> GetBookEntityById(int id)
        {
            return await _db.BookList.FindAsync(id);
        }

        public async Task AddBook(Book book)
        {
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
                existingBook.CoverImagePath = book.CoverImagePath;
                _db.BookList.Update(existingBook);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteBook(int id)
        {
            var book = await _db.BookList.FindAsync(id);
            if (book != null)
            {
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
                BookIsbn = book.BookIsbn,
                CoverImagePath = book.CoverImagePath ?? "" // Add fallback for null
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
                IsBookmarked = true,
                CoverImagePath = x.Book.CoverImagePath ?? "" // Add fallback for null
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
                Quantity = x.CartBook.Quantity,
                CoverImagePath = x.Book.CoverImagePath ?? "" // Add fallback for null
            }).ToList() : new List<ShowBook>();
            return result;
        }

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

        public async Task CreateOrder(string userId, List<ShowBook> cartItems)
        {
            if (cartItems == null || !cartItems.Any()) return;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = cartItems.Sum(item => item.Quantity * item.BookPrice),
                Status = "Pending"
            };

            var maxOrderId = await _db.Orders.MaxAsync(o => (int?)o.OrderId) ?? 0;
            order.OrderId = maxOrderId + 1;

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.BookPrice
                };
                _db.OrderItems.Add(orderItem);
            }

            var userCartItems = await _db.CartBooks.Where(c => c.UserId == userId).ToListAsync();
            if (userCartItems.Any()) _db.CartBooks.RemoveRange(userCartItems);

            await _db.SaveChangesAsync();
        }

        public async Task<List<AdminOrderViewModel>> GetAllOrders()
        {
            var orders = await _db.Orders
                .Join(_db.Users,
                    o => o.UserId,
                    u => u.Id,
                    (o, u) => new { Order = o, User = u })
                .ToListAsync();

            var orderViewModels = new List<AdminOrderViewModel>();
            foreach (var order in orders)
            {
                var orderItems = await _db.OrderItems
                    .Where(oi => oi.OrderId == order.Order.OrderId)
                    .Join(_db.BookList,
                        oi => oi.BookId,
                        b => b.BookId,
                        (oi, b) => new OrderItemViewModel
                        {
                            BookId = oi.BookId,
                            BookTitle = b.BookTitle ?? "",
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice
                        })
                    .ToListAsync();

                orderViewModels.Add(new AdminOrderViewModel
                {
                    OrderId = order.Order.OrderId,
                    UserId = order.Order.UserId,
                    UserEmail = order.User.Email,
                    OrderDate = order.Order.OrderDate,
                    TotalPrice = order.Order.TotalPrice,
                    Status = order.Order.Status,
                    OrderItems = orderItems
                });
            }

            return orderViewModels;
        }

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Announcement>> GetAllAnnouncements()
        {
            return await _db.Announcements.OrderByDescending(a => a.PostedDate).ToListAsync();
        }

        public async Task AddAnnouncement(Announcement announcement)
        {
            var maxId = await _db.Announcements.MaxAsync(a => (int?)a.AnnouncementId) ?? 0;
            announcement.AnnouncementId = maxId + 1;
            announcement.PostedDate = DateTime.UtcNow;
            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAnnouncement(Announcement announcement)
        {
            var existingAnnouncement = await _db.Announcements.FindAsync(announcement.AnnouncementId);
            if (existingAnnouncement != null)
            {
                existingAnnouncement.Title = announcement.Title;
                existingAnnouncement.Content = announcement.Content;
                existingAnnouncement.IsActive = announcement.IsActive;
                _db.Announcements.Update(existingAnnouncement);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAnnouncement(int announcementId)
        {
            var announcement = await _db.Announcements.FindAsync(announcementId);
            if (announcement != null)
            {
                _db.Announcements.Remove(announcement);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<AdminReportViewModel> GenerateAdminReport()
        {
            var report = new AdminReportViewModel();

            var orders = await _db.Orders.ToListAsync();
            report.TotalSales = orders.Sum(o => o.TotalPrice);
            report.TotalOrders = orders.Count;
            report.TotalUsers = await _db.Users.CountAsync();

            var books = await _db.BookList.ToDictionaryAsync(b => b.BookId, b => b.BookTitle);

            var topSellingBooksQuery = await _db.OrderItems
                .GroupBy(oi => oi.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(tsb => tsb.TotalQuantitySold)
                .Take(5)
                .ToListAsync();

            var topSellingBooks = topSellingBooksQuery.Select(tsb => new TopSellingBook
            {
                BookId = tsb.BookId,
                BookTitle = books.ContainsKey(tsb.BookId) ? books[tsb.BookId]! : "Unknown",
                TotalQuantitySold = tsb.TotalQuantitySold,
                TotalRevenue = tsb.TotalRevenue
            }).ToList();

            report.TopSellingBooks = topSellingBooks;

            var users = await _db.Users.ToDictionaryAsync(u => u.Id, u => u.Email);

            var userActivityQuery = await _db.Orders
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(ua => ua.OrderCount)
                .Take(5)
                .ToListAsync();

            var userActivity = userActivityQuery.Select(ua => new UserActivity
            {
                UserId = ua.UserId!,
                UserEmail = !string.IsNullOrEmpty(ua.UserId) && users.ContainsKey(ua.UserId) ? users[ua.UserId]! : "Unknown",
                OrderCount = ua.OrderCount
            }).ToList();

            report.UserActivity = userActivity;

            return report;
        }
    }
}