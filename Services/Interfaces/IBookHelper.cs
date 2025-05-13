using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Models.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GyanGanga.Web.Services.Interfaces
{
    public interface IBookHelper
    {
        Task<List<ShowBook>> GetAllBooks();
        Task<Book?> GetBookEntityById(int id); // Changed to Book?
        Task AddBook(Book book);
        Task UpdateBook(Book book);
        Task DeleteBook(int id);
        Task<ShowBookDetails?> GetBookById(int id);
        Task SaveBook(int bookId, string userId);
        Task Unbookmark(int bookId, string userId);
        Task<bool> IsBookmarked(int bookId, string userId);
        Task AddToCart(int bookId, string userId, int quantity);
        Task UpdateCartItemQuantity(int bookId, string userId, int quantity);
        Task RemoveFromCart(int bookId, string userId);
        Task<List<ShowBook>> GetSavedBooks(string userId);
        Task<List<ShowBook>> GetCartBooks(string userId);
        Task<List<(CartBook CartItem, Book Book)>> GetAllCartItems();
        Task RemoveUserCartAndBookmarks(string userId);
        Task CreateOrder(string userId, List<ShowBook> cartItems);
        Task<List<AdminOrderViewModel>> GetAllOrders();
        Task UpdateOrderStatus(int orderId, string status);
        Task<List<Announcement>> GetAllAnnouncements();
        Task AddAnnouncement(Announcement announcement);
        Task UpdateAnnouncement(Announcement announcement);
        Task DeleteAnnouncement(int announcementId);
        Task<AdminReportViewModel> GenerateAdminReport();
    }
}