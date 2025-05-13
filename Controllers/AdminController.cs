using GyanGanga.Web.Data; // Added
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

namespace GyanGanga.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBookHelper _bookHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyDB _db;

        public AdminController(IBookHelper bookHelper, UserManager<ApplicationUser> userManager, MyDB db)
        {
            _bookHelper = bookHelper;
            _userManager = userManager;
            _db = db;
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

        public async Task<IActionResult> Books()
        {
            var books = await _bookHelper.GetAllBooks();
            return View(books);
        }

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookHelper.AddBook(book);
                return RedirectToAction("Books");
            }
            return View(book);
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
        public async Task<IActionResult> EditBook(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookHelper.UpdateBook(book);
                return RedirectToAction("Books");
            }
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookHelper.DeleteBook(id);
            return RedirectToAction("Books");
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User";
                userViewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = role
                });
            }

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

        public async Task<IActionResult> Orders()
        {
            var orders = await _bookHelper.GetAllOrders();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            await _bookHelper.UpdateOrderStatus(orderId, status);
            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> Announcements()
        {
            var announcements = await _bookHelper.GetAllAnnouncements();
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