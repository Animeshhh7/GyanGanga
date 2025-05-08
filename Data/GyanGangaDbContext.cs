using Microsoft.EntityFrameworkCore; 
using GyanGanga.Web.Models.Entities; 
 
namespace GyanGanga.Web.Data 
{ 
    public class GyanGangaDbContext : DbContext 
    { 
        public GyanGangaDbContext(DbContextOptions<GyanGangaDbContext> options) 
            : base(options) 
        { 
        } 
 
        public DbSet<Book> Books { get; set; } 
 
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            modelBuilder.Entity<Book>().ToTable("books", "public"); 
            modelBuilder.Entity<Book>().Property(b => b.Id).HasColumnName("id"); 
            modelBuilder.Entity<Book>().Property(b => b.Title).HasColumnName("title"); 
            modelBuilder.Entity<Book>().Property(b => b.Author).HasColumnName("author"); 
            modelBuilder.Entity<Book>().Property(b => b.Genre).HasColumnName("genre"); 
            modelBuilder.Entity<Book>().Property(b => b.Price).HasColumnName("price"); 
            modelBuilder.Entity<Book>().Property(b => b.Format).HasColumnName("format"); 
            modelBuilder.Entity<Book>().Property(b => b.Stock).HasColumnName("stock"); 
            modelBuilder.Entity<Book>().Property(b => b.ISBN).HasColumnName("isbn"); 
        } 
    } 
} 
