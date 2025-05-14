using GyanGanga.Web.Models;
using GyanGanga.Web.Models.Classes;
using GyanGanga.Web.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GyanGanga.Web.Data
{
    public class MyDB : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public MyDB(DbContextOptions<MyDB> options) : base(options) { }

        public DbSet<Book> BookList { get; set; }
        public DbSet<Bookmarks> Bookmarks { get; set; }
        public DbSet<CartBook> CartBooks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Custom entities
            builder.Entity<Book>().ToTable("book_list");
            builder.Entity<Book>().HasKey(b => b.BookId);
            builder.Entity<Book>().Property(b => b.BookId).HasColumnName("book_id");
            builder.Entity<Book>().Property(b => b.BookTitle).HasColumnName("book_title");
            builder.Entity<Book>().Property(b => b.BookAuthor).HasColumnName("book_author");
            builder.Entity<Book>().Property(b => b.BookGenre).HasColumnName("book_genre");
            builder.Entity<Book>().Property(b => b.BookPrice).HasColumnName("book_price");
            builder.Entity<Book>().Property(b => b.BookIsbn).HasColumnName("book_isbn");
            builder.Entity<Book>().Property(b => b.BookFormat).HasColumnName("book_format");
            builder.Entity<Book>().Property(b => b.BookStock).HasColumnName("book_stock");
            builder.Entity<Book>().Property(b => b.Rating).HasColumnName("rating");
            builder.Entity<Book>().Property(b => b.CoverImagePath).HasColumnName("cover_image_path");

            builder.Entity<Bookmarks>().ToTable("bookmarks");
            builder.Entity<Bookmarks>().Property(s => s.BookId).HasColumnName("book_id");
            builder.Entity<Bookmarks>().Property(s => s.UserId).HasColumnName("user_id");
            builder.Entity<Bookmarks>().HasKey(s => new { s.BookId, s.UserId });

            builder.Entity<CartBook>().ToTable("cart_books");
            builder.Entity<CartBook>().Property(c => c.BookId).HasColumnName("book_id");
            builder.Entity<CartBook>().Property(c => c.UserId).HasColumnName("user_id");
            builder.Entity<CartBook>().Property(c => c.Quantity).HasColumnName("quantity");
            builder.Entity<CartBook>().HasKey(c => new { c.BookId, c.UserId });

            builder.Entity<Order>().ToTable("orders");
            builder.Entity<Order>().HasKey(o => o.OrderId);
            builder.Entity<Order>().Property(o => o.OrderId).HasColumnName("order_id");
            builder.Entity<Order>().Property(o => o.UserId).HasColumnName("user_id");
            builder.Entity<Order>().Property(o => o.OrderDate).HasColumnName("order_date");
            builder.Entity<Order>().Property(o => o.TotalPrice).HasColumnName("total_price");
            builder.Entity<Order>().Property(o => o.Status).HasColumnName("status");

            builder.Entity<OrderItem>().ToTable("order_items");
            builder.Entity<OrderItem>().Property(oi => oi.OrderId).HasColumnName("order_id");
            builder.Entity<OrderItem>().Property(oi => oi.BookId).HasColumnName("book_id");
            builder.Entity<OrderItem>().Property(oi => oi.Quantity).HasColumnName("quantity");
            builder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasColumnName("unit_price");
            builder.Entity<OrderItem>().HasKey(oi => new { oi.OrderId, oi.BookId });

            // Configure the one-to-many relationship between Order and OrderItem
            builder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // Configure the one-to-many relationship between OrderItem and Book
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany()
                .HasForeignKey(oi => oi.BookId);

            builder.Entity<Announcement>().ToTable("announcements");
            builder.Entity<Announcement>().HasKey(a => a.AnnouncementId);
            builder.Entity<Announcement>().Property(a => a.AnnouncementId).HasColumnName("announcement_id");
            builder.Entity<Announcement>().Property(a => a.Title).HasColumnName("title");
            builder.Entity<Announcement>().Property(a => a.Content).HasColumnName("content");
            builder.Entity<Announcement>().Property(a => a.PostedDate).HasColumnName("posted_date");
            builder.Entity<Announcement>().Property(a => a.IsActive).HasColumnName("is_active");

            // Configure Ratings
            builder.Entity<Rating>().ToTable("ratings");
            builder.Entity<Rating>().Property(r => r.BookId).HasColumnName("book_id");
            builder.Entity<Rating>().Property(r => r.UserId).HasColumnName("user_id");
            builder.Entity<Rating>().Property(r => r.Value).HasColumnName("value");
            builder.Entity<Rating>().HasKey(r => new { r.BookId, r.UserId });

            builder.Entity<Rating>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId);

            // Configure Reviews
            builder.Entity<Review>().ToTable("reviews");
            builder.Entity<Review>().HasKey(r => r.Id);
            builder.Entity<Review>().Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Entity<Review>().Property(r => r.BookId).HasColumnName("book_id");
            builder.Entity<Review>().Property(r => r.UserId).HasColumnName("user_id");
            builder.Entity<Review>().Property(r => r.Content).HasColumnName("content").HasMaxLength(50);
            builder.Entity<Review>().Property(r => r.PostedDate).HasColumnName("posted_date");

            builder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId);

            // Identity tables with snake_case column mappings
            builder.Entity<IdentityRole>().ToTable("roles");
            builder.Entity<IdentityRole>().Property(r => r.Id).HasColumnName("id");
            builder.Entity<IdentityRole>().Property(r => r.Name).HasColumnName("name");
            builder.Entity<IdentityRole>().Property(r => r.NormalizedName).HasColumnName("normalized_name");
            builder.Entity<IdentityRole>().Property(r => r.ConcurrencyStamp).HasColumnName("concurrency_stamp");

            builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
            builder.Entity<IdentityUserRole<string>>().Property(ur => ur.UserId).HasColumnName("user_id");
            builder.Entity<IdentityUserRole<string>>().Property(ur => ur.RoleId).HasColumnName("role_id");

            builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
            builder.Entity<IdentityUserClaim<string>>().Property(uc => uc.Id).HasColumnName("id");
            builder.Entity<IdentityUserClaim<string>>().Property(uc => uc.UserId).HasColumnName("user_id");
            builder.Entity<IdentityUserClaim<string>>().Property(uc => uc.ClaimType).HasColumnName("claim_type");
            builder.Entity<IdentityUserClaim<string>>().Property(uc => uc.ClaimValue).HasColumnName("claim_value");

            builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
            builder.Entity<IdentityUserLogin<string>>().Property(ul => ul.UserId).HasColumnName("user_id");
            builder.Entity<IdentityUserLogin<string>>().Property(ul => ul.LoginProvider).HasColumnName("login_provider");
            builder.Entity<IdentityUserLogin<string>>().Property(ul => ul.ProviderKey).HasColumnName("provider_key");
            builder.Entity<IdentityUserLogin<string>>().Property(ul => ul.ProviderDisplayName).HasColumnName("provider_display_name");

            builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
            builder.Entity<IdentityUserToken<string>>().Property(ut => ut.UserId).HasColumnName("user_id");
            builder.Entity<IdentityUserToken<string>>().Property(ut => ut.LoginProvider).HasColumnName("login_provider");
            builder.Entity<IdentityUserToken<string>>().Property(ut => ut.Name).HasColumnName("name");
            builder.Entity<IdentityUserToken<string>>().Property(ut => ut.Value).HasColumnName("value");

            builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
            builder.Entity<IdentityRoleClaim<string>>().Property(rc => rc.Id).HasColumnName("id");
            builder.Entity<IdentityRoleClaim<string>>().Property(rc => rc.RoleId).HasColumnName("role_id");
            builder.Entity<IdentityRoleClaim<string>>().Property(rc => rc.ClaimType).HasColumnName("claim_type");
            builder.Entity<IdentityRoleClaim<string>>().Property(rc => rc.ClaimValue).HasColumnName("claim_value");

            builder.Entity<ApplicationUser>().ToTable("users");
            builder.Entity<ApplicationUser>().Property(u => u.Id).HasColumnName("id");
            builder.Entity<ApplicationUser>().Property(u => u.UserName).HasColumnName("user_name");
            builder.Entity<ApplicationUser>().Property(u => u.NormalizedUserName).HasColumnName("normalized_user_name");
            builder.Entity<ApplicationUser>().Property(u => u.Email).HasColumnName("email");
            builder.Entity<ApplicationUser>().Property(u => u.NormalizedEmail).HasColumnName("normalized_email");
            builder.Entity<ApplicationUser>().Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
            builder.Entity<ApplicationUser>().Property(u => u.PasswordHash).HasColumnName("password_hash");
            builder.Entity<ApplicationUser>().Property(u => u.SecurityStamp).HasColumnName("security_stamp");
            builder.Entity<ApplicationUser>().Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            builder.Entity<ApplicationUser>().Property(u => u.PhoneNumber).HasColumnName("phone_number");
            builder.Entity<ApplicationUser>().Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            builder.Entity<ApplicationUser>().Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            builder.Entity<ApplicationUser>().Property(u => u.LockoutEnd).HasColumnName("lockout_end");
            builder.Entity<ApplicationUser>().Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
            builder.Entity<ApplicationUser>().Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");
            builder.Entity<ApplicationUser>().Property(u => u.FirstName).HasColumnName("first_name");
            builder.Entity<ApplicationUser>().Property(u => u.LastName).HasColumnName("last_name");
        }
    }
}