using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GyanGanga.Web.Data;
using GyanGanga.Web.Models.Views;

namespace GyanGanga.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookHelper _bookHelper;
        private readonly MyDB _db;

        public BooksController(IBookHelper bookHelper, MyDB db)
        {
            _bookHelper = bookHelper;
            _db = db;
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

            // Fetch review counts for all books in the query
            var bookIds = booksQuery.Select(b => b.BookId).ToList();
            var reviewCounts = await _db.Reviews
                .Where(r => bookIds.Contains(r.BookId))
                .GroupBy(r => r.BookId)
                .Select(g => new { BookId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.BookId, g => g.Count);

            // Assign review counts to each book
            foreach (var book in booksQuery)
            {
                book.ReviewCount = reviewCounts.ContainsKey(book.BookId) ? reviewCounts[book.BookId] : 0;
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

        [Authorize]
        public async Task<IActionResult> MyPurchases()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderViewModels = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                var orderViewModel = new OrderViewModel
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId ?? string.Empty,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status ?? string.Empty,
                    OrderItems = new List<OrderItemViewModel>()
                };

                foreach (var item in order.OrderItems)
                {
                    var userRating = await _bookHelper.GetUserRating(item.BookId, userId);
                    var userReview = await _bookHelper.GetUserReview(item.BookId, userId);

                    orderViewModel.OrderItems.Add(new OrderItemViewModel
                    {
                        BookId = item.BookId,
                        BookTitle = item.Book.BookTitle ?? string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        UserRating = userRating,
                        UserReview = userReview
                    });
                }

                orderViewModels.Add(orderViewModel);
            }

            return View(orderViewModels);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ClearPurchaseHistory()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _bookHelper.ClearPurchaseHistory(userId);
            TempData["Success"] = "Purchase history cleared successfully!";
            return RedirectToAction("MyPurchases");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitRating(int bookId, decimal rating)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the user has purchased the book
            var hasPurchased = await _bookHelper.HasPurchasedBook(userId, bookId);
            if (!hasPurchased)
            {
                TempData["Error"] = "You can only rate books you have purchased.";
                return RedirectToAction("MyPurchases");
            }

            try
            {
                await _bookHelper.SubmitRating(bookId, userId, rating);
                TempData["Success"] = "Rating submitted successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("MyPurchases");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitReview(int bookId, string review)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the user has purchased the book
            var hasPurchased = await _bookHelper.HasPurchasedBook(userId, bookId);
            if (!hasPurchased)
            {
                TempData["Error"] = "You can only review books you have purchased.";
                return RedirectToAction("MyPurchases");
            }

            try
            {
                await _bookHelper.SubmitReview(bookId, userId, review);
                TempData["Success"] = "Review submitted successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("MyPurchases");
        }

        public async Task<IActionResult> Reviews(int id)
        {
            // Fetch the book details
            var book = await _db.BookList.FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            // Fetch all reviews for the book
            var reviews = await _db.Reviews
                .Where(r => r.BookId == id)
                .Join(
                    _db.Users,
                    r => r.UserId,
                    u => u.Id,
                    (r, u) => new ReviewViewModel
                    {
                        UserName = u.UserName ?? "Anonymous",
                        Content = r.Content,
                        PostedDate = r.PostedDate
                    })
                .OrderByDescending(r => r.PostedDate)
                .ToListAsync();

            var viewModel = new BookReviewsViewModel
            {
                BookId = book.BookId,
                BookTitle = book.BookTitle ?? string.Empty,
                Reviews = reviews
            };

            return View(viewModel);
        }

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Order #{orderId} status updated to {status} successfully!";
            }
        }
    }
}