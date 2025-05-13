using GyanGanga.Web.Models;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Models.Identity;
using GyanGanga.Web.Models.Views; // For AdminUserViewModel
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

        public AdminController(IBookHelper bookHelper, UserManager<ApplicationUser> userManager)
        {
            _bookHelper = bookHelper;
            _userManager = userManager;
        }

        // Admin Dashboard
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

        // Book Management: List Books
        public async Task<IActionResult> Books()
        {
            var books = await _bookHelper.GetAllBooks();
            return View(books);
        }

        // Book Management: Add Book (GET)
        public IActionResult AddBook()
        {
            return View();
        }

        // Book Management: Add Book (POST)
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

        // Book Management: Edit Book (GET)
        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _bookHelper.GetBookEntityById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // Book Management: Edit Book (POST)
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

        // Book Management: Delete Book
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookHelper.DeleteBook(id);
            return RedirectToAction("Books");
        }

        // User Management: List Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User"; // Default to "User" if no role
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

        // User Management: Delete User
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

        // Cart Management: List All Cart Items
        public async Task<IActionResult> Carts()
        {
            var cartItems = await _bookHelper.GetAllCartItems();
            return View(cartItems);
        }

        // Cart Management: Delete Cart Item
        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int bookId, string userId)
        {
            await _bookHelper.RemoveFromCart(bookId, userId);
            return RedirectToAction("Carts");
        }
    }
}