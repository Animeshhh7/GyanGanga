using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace GyanGanga.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookHelper _bookHelper;

        public BooksController(IBookHelper bookHelper)
        {
            _bookHelper = bookHelper;
        }

        public async Task<IActionResult> Index(string search, string sort, string genre, string format, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            const int pageSize = 5;
            var booksQuery = await _bookHelper.GetAllBooks();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                booksQuery = booksQuery.Where(b =>
                    (!string.IsNullOrEmpty(b.BookTitle) && b.BookTitle.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(b.BookAuthor) && b.BookAuthor.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(b.BookIsbn) && b.BookIsbn.ToLower().Contains(search))).ToList();
            }

            if (!string.IsNullOrEmpty(genre))
            {
                booksQuery = booksQuery.Where(b => b.BookGenre == genre).ToList();
            }

            if (!string.IsNullOrEmpty(format))
            {
                booksQuery = booksQuery.Where(b => b.BookFormat == format).ToList();
            }

            if (minPrice.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.BookPrice >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.BookPrice <= maxPrice.Value).ToList();
            }

            switch (sort)
            {
                case "title_asc":
                    booksQuery = booksQuery.OrderBy(b => b.BookTitle).ToList();
                    break;
                case "title_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.BookTitle).ToList();
                    break;
                case "price_asc":
                    booksQuery = booksQuery.OrderBy(b => b.BookPrice).ToList();
                    break;
                case "price_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.BookPrice).ToList();
                    break;
                case "popularity_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.Rating).ToList();
                    break;
                default:
                    booksQuery = booksQuery.OrderBy(b => b.BookId).ToList();
                    break;
            }

            var totalBooks = booksQuery.Count;
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));
            var books = booksQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                foreach (var book in books)
                {
                    book.IsBookmarked = await _bookHelper.IsBookmarked(book.BookId, userId);
                }
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Genre = genre;
            ViewBag.Format = format;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookHelper.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveBook(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            await _bookHelper.SaveBook(id, userId);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unbookmark(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            await _bookHelper.Unbookmark(id, userId);
            return RedirectToAction("Bookmarks");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UnbookmarkFromBooks(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            await _bookHelper.Unbookmark(id, userId);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            await _bookHelper.AddToCart(id, userId, quantity);
            return RedirectToAction("Cart");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateCartItemQuantity(int id, int quantity)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            await _bookHelper.UpdateCartItemQuantity(id, userId, quantity);
            return RedirectToAction("Cart");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            await _bookHelper.RemoveFromCart(id, userId);
            return RedirectToAction("Cart");
        }

        [Authorize]
        public async Task<IActionResult> Bookmarks()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var savedBooks = await _bookHelper.GetSavedBooks(userId);
            foreach (var book in savedBooks)
            {
                book.IsBookmarked = await _bookHelper.IsBookmarked(book.BookId, userId);
            }
            return View(savedBooks);
        }

        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var cartBooks = await _bookHelper.GetCartBooks(userId);
            ViewBag.TotalItems = cartBooks.Sum(b => b.Quantity);
            ViewBag.TotalPrice = cartBooks.Sum(b => b.Quantity * b.BookPrice);
            return View(cartBooks);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartBooks = await _bookHelper.GetCartBooks(userId);
            if (cartBooks == null || !cartBooks.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            await _bookHelper.CreateOrder(userId, cartBooks);
            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Cart");
        }
    }
}