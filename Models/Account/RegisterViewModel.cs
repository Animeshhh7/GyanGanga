using System.ComponentModel.DataAnnotations; 
 
namespace GyanGanga.Web.Models.Account 
{ 
    public class RegisterViewModel 
    { 
        [Required(ErrorMessage = "First name is required")] 
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")] 
        public string? FirstName { get; set; } 
 
        [Required(ErrorMessage = "Last name is required")] 
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")] 
        public string? LastName { get; set; } 
 
        [Required(ErrorMessage = "Email is required")] 
        [EmailAddress(ErrorMessage = "Invalid email address")] 
        public string? Email { get; set; } 
 
        [Phone(ErrorMessage = "Invalid phone number")] 
        public string? PhoneNumber { get; set; } 
 
        [Required(ErrorMessage = "Password is required")] 
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")] 
        public string? Password { get; set; } 
    } 
} 
