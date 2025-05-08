using GyanGanga.Web.Services.Interfaces; 
using Microsoft.AspNetCore.Mvc; 
using System.Threading.Tasks; 
 
namespace GyanGanga.Web.Controllers 
{ 
    public class BooksController : Controller 
    { 
        private readonly IBookService _bookService; 
 
        public BooksController(IBookService bookService) 
        { 
            _bookService = bookService; 
        } 
 
        public async Task<IActionResult> Index() 
        { 
            var books = await _bookService.GetBooksAsync(); 
            return View(books); 
        } 
 
        public async Task<IActionResult> Details(int id) 
        { 
            var book = await _bookService.GetBookByIdAsync(id); 
            if (book == null) 
            { 
                return NotFound(); 
            } 
            return View(book); 
        } 
    } 
} 
