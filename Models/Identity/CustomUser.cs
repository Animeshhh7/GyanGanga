namespace GyanGanga.Web.Models.Identity 
{ 
    public class CustomUser 
    { 
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string PhoneNumber { get; set; } = string.Empty; 
        public string UserName { get; set; } = string.Empty; 
        public string PasswordHash { get; set; } = string.Empty; 
        public string SecurityStamp { get; set; } = string.Empty; 
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString(); 
        public int AccessFailedCount { get; set; } 
        public bool EmailConfirmed { get; set; } 
        public bool PhoneNumberConfirmed { get; set; } 
        public bool TwoFactorEnabled { get; set; } 
        public bool LockoutEnabled { get; set; } 
        public DateTimeOffset? LockoutEnd { get; set; } 
    } 
} 
