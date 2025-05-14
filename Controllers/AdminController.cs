using GyanGanga.Web.Data;
using GyanGanga.Web.Models;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Models.Identity;
using GyanGanga.Web.Models.Views;
using GyanGanga.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace GyanGanga.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBookHelper _bookHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyDB _db;
        private readonly IWebHostEnvironment _environment;

        public AdminController(IBookHelper bookHelper, UserManager<ApplicationUser> userManager, MyDB db, IWebHostEnvironment environment)
        {
            _bookHelper = bookHelper;
            _userManager = userManager;
            _db = db;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookHelper.GetAllBooks();
            var users = await _userManager.Users.ToListAsync();
            var cartItems = await _bookHelper.GetAllCartItems();

            ViewBag.TotalBooks = books.Count;
            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalCartItems = cartItems.Count();

            return View();
        }

        public async Task<IActionResult> Books(string search, string genre)
        {
            var books = await _bookHelper.GetAllBooks();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                books = books.Where(b =>
                    (!string.IsNullOrEmpty(b.BookTitle) && b.BookTitle.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(b.BookAuthor) && b.BookAuthor.ToLower().Contains(search))
                ).ToList();
            }

            // Apply genre filter
            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.BookGenre == genre).ToList();
            }

            // Pass filter values to ViewBag for form persistence
            ViewBag.Search = search;
            ViewBag.Genre = genre;

            return View(books);
        }

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(Book book, IFormFile coverImage)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }

            // Handle image upload
            if (coverImage != null && coverImage.Length > 0)
            {
                // Ensure the uploads directory exists
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique file name to avoid conflicts
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(coverImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await coverImage.CopyToAsync(fileStream);
                }

                // Store the relative path in the database
                book.CoverImagePath = "/uploads/" + fileName;
            }

            // Save the book to the database
            await _bookHelper.AddBook(book);
            return RedirectToAction("Books");
        }

        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _bookHelper.GetBookEntityById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(Book book, IFormFile coverImage)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }

            // Handle image upload if a new file is provided
            if (coverImage != null && coverImage.Length > 0)
            {
                // Ensure the uploads directory exists
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique file name to avoid conflicts
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(coverImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await coverImage.CopyToAsync(fileStream);
                }

                // Update the book's CoverImagePath with the new file path
                book.CoverImagePath = "/uploads/" + fileName;
            }
            // If no new image is uploaded, the existing CoverImagePath will remain unchanged

            await _bookHelper.UpdateBook(book);
            return RedirectToAction("Books");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookHelper.DeleteBook(id);
                TempData["Success"] = "Book deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete the book. It may be referenced in bookmarks or cart items.";
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Books");
        }

        public async Task<IActionResult> Users(string search, string role)
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "User";
                userViewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = userRole
                });
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                userViewModels = userViewModels.Where(u =>
                    (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(search))
                ).ToList();
            }

            // Apply role filter
            if (!string.IsNullOrEmpty(role))
            {
                userViewModels = userViewModels.Where(u => u.Role == role).ToList();
            }

            // Pass filter values to ViewBag for form persistence
            ViewBag.Search = search;
            ViewBag.Role = role;

            return View(userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _bookHelper.RemoveUserCartAndBookmarks(id);
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Carts()
        {
            var cartItems = await _bookHelper.GetAllCartItems();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int bookId, string userId)
        {
            await _bookHelper.RemoveFromCart(bookId, userId);
            return RedirectToAction("Carts");
        }

        public async Task<IActionResult> Orders(string search, string status)
        {
            var orders = await _bookHelper.GetAllOrders();

            // Apply search filter (by UserEmail)
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                orders = orders.Where(o =>
                    !string.IsNullOrEmpty(o.UserEmail) && o.UserEmail.ToLower().Contains(search)
                ).ToList();
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status == status).ToList();
            }

            // Pass filter values to ViewBag for form persistence
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            await _bookHelper.UpdateOrderStatus(orderId, status);
            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> Announcements(string search, string status)
        {
            var announcements = await _bookHelper.GetAllAnnouncements();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                announcements = announcements.Where(a =>
                    (!string.IsNullOrEmpty(a.Title) && a.Title.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(a.Content) && a.Content.ToLower().Contains(search))
                ).ToList();
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status.ToLower() == "active";
                announcements = announcements.Where(a => a.IsActive == isActive).ToList();
            }

            // Pass filter values to ViewBag for form persistence
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(announcements);
        }

        public IActionResult CreateAnnouncement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                await _bookHelper.AddAnnouncement(announcement);
                return RedirectToAction("Announcements");
            }
            return View(announcement);
        }

        public async Task<IActionResult> EditAnnouncement(int id)
        {
            var announcement = await _db.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            return View(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> EditAnnouncement(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                await _bookHelper.UpdateAnnouncement(announcement);
                return RedirectToAction("Announcements");
            }
            return View(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            await _bookHelper.DeleteAnnouncement(id);
            return RedirectToAction("Announcements");
        }

        public async Task<IActionResult> Reports()
        {
            var report = await _bookHelper.GenerateAdminReport();
            return View(report);
        }
    }
}