using GyanGanga.Web.Models.Classes; 
using GyanGanga.Web.Services.Interfaces; 
using Microsoft.AspNetCore.Mvc; 
using System.Threading.Tasks; 
namespace GyanGanga.Web.Controllers { 
    public class HomeController : Controller { 
        private readonly IBookHelper _bookHelper; 
        public HomeController(IBookHelper bookHelper) { 
            _bookHelper = bookHelper; 
        } 
        public async Task<IActionResult> Index() { 
            var books = await _bookHelper.GetAllBooks(); 
            return View(books); 
        } 
    } 
} 
