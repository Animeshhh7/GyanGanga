using GyanGanga.Web.Models;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Models.Identity;
using Microsoft.AspNetCore.Identity; // Added to resolve IdentityRole
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GyanGanga.Web.Data
{
    public class MyDB : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<Book> BookList { get; set; }
        public DbSet<Bookmarks> Bookmarks { get; set; }
        public DbSet<CartBook> CartBooks { get; set; }

        public MyDB(DbContextOptions<MyDB> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    var columnName = Regex.Replace(property.Name, "([A-Z])", "_$1").ToLower().TrimStart('_');
                    property.SetColumnName(columnName);
                }
            }
            builder.Entity<ApplicationUser>().ToTable("users");
            builder.Entity<IdentityRole>().ToTable("roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
            builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
            builder.Entity<Book>().ToTable("book_list");
            builder.Entity<Book>().HasKey(b => b.BookId);
            builder.Entity<Bookmarks>().ToTable("bookmarks");
            builder.Entity<Bookmarks>().Property(s => s.BookId).HasColumnName("book_id");
            builder.Entity<Bookmarks>().Property(s => s.UserId).HasColumnName("user_id");
            builder.Entity<Bookmarks>().HasKey(s => new { s.BookId, s.UserId });
            builder.Entity<CartBook>().ToTable("cart_books");
            builder.Entity<CartBook>().Property(c => c.BookId).HasColumnName("book_id");
            builder.Entity<CartBook>().Property(c => c.UserId).HasColumnName("user_id");
            builder.Entity<CartBook>().Property(c => c.Quantity).HasColumnName("quantity");
            builder.Entity<CartBook>().HasKey(c => new { c.BookId, c.UserId });
        }
    }
}