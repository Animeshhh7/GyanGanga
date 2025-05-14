using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace GyanGanga.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookHelper _bookHelper;

        public BooksController(IBookHelper bookHelper)
        {
            _bookHelper = bookHelper;
        }

        public async Task<IActionResult> Index(string search, string sort, string genre, string format, decimal? minPrice, decimal? maxPrice)
        {
            var booksQuery = await _bookHelper.GetAllBooks();

            // Apply filters
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

            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                foreach (var book in booksQuery)
                {
                    book.IsBookmarked = await _bookHelper.IsBookmarked(book.BookId, userId);
                }
            }

            // Pass filter parameters to the view for form persistence
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Genre = genre;
            ViewBag.Format = format;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(booksQuery); // Return all books without pagination
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

            // Pass cart details to TempData before clearing the cart
            TempData["OrderItems"] = System.Text.Json.JsonSerializer.Serialize(cartBooks);
            TempData["TotalItems"] = cartBooks.Sum(b => b.Quantity);
            TempData["TotalPrice"] = cartBooks.Sum(b => b.Quantity * b.BookPrice).ToString("N2"); // Convert to string
            TempData["TotalDiscount"] = 0m.ToString("N2"); // Convert to string
            TempData["FinalPrice"] = cartBooks.Sum(b => b.Quantity * b.BookPrice).ToString("N2"); // Convert to string

            await _bookHelper.CreateOrder(userId, cartBooks);
            return RedirectToAction("OrderConfirmation");
        }

        [Authorize]
        public IActionResult OrderConfirmation()
        {
            // Retrieve order details from TempData
            var orderItemsJson = TempData["OrderItems"]?.ToString();
            var orderItems = string.IsNullOrEmpty(orderItemsJson)
                ? new List<ShowBook>()
                : System.Text.Json.JsonSerializer.Deserialize<List<ShowBook>>(orderItemsJson);

            ViewBag.TotalItems = TempData["TotalItems"] ?? 0;
            ViewBag.TotalPrice = decimal.Parse(TempData["TotalPrice"]?.ToString() ?? "0");
            ViewBag.TotalDiscount = decimal.Parse(TempData["TotalDiscount"]?.ToString() ?? "0");
            ViewBag.FinalPrice = decimal.Parse(TempData["FinalPrice"]?.ToString() ?? "0");

            return View(orderItems);
        }
    }
}