using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc; 
using System.Threading.Tasks; 
using GyanGanga.Web.Models.Account; 
using GyanGanga.Web.Models.Identity; 
using System; 
namespace GyanGanga.Web.Controllers { 
    public class AccountController : Controller { 
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly SignInManager<ApplicationUser> _signInManager; 
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) { 
            _userManager = userManager; 
            _signInManager = signInManager; 
        } 
        public IActionResult Register() { 
            return View(new RegisterViewModel()); 
        } 
        [HttpPost] 
        public async Task<IActionResult> Register(RegisterViewModel model) { 
            if (!ModelState.IsValid) { 
                return View(model); 
            } 
            var user = new ApplicationUser { 
                Id = Guid.NewGuid().ToString(), 
                UserName = model.Email!, 
                Email = model.Email!, 
                FirstName = model.FirstName, 
                LastName = model.LastName, 
                PhoneNumber = model.PhoneNumber 
            }; 
            var result = await _userManager.CreateAsync(user, model.Password!); 
            if (result.Succeeded) { 
                await _signInManager.SignInAsync(user, false); 
                return RedirectToAction("Index", "Home"); 
            } 
            foreach (var error in result.Errors) { 
                ModelState.AddModelError(string.Empty, error.Description); 
            } 
            return View(model); 
        } 
        public IActionResult Login() { 
            return View(new LoginViewModel()); 
        } 
        [HttpPost] 
        public async Task<IActionResult> Login(LoginViewModel model) { 
            if (!ModelState.IsValid) { 
                return View(model); 
            } 
            var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, false, false); 
            if (result.Succeeded) { 
                return RedirectToAction("Index", "Home"); 
            } 
            ModelState.AddModelError(string.Empty, "Invalid email or password"); 
            return View(model); 
        } 
        public async Task<IActionResult> Logout() { 
            await _signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "Home"); 
        } 
    } 
} 
