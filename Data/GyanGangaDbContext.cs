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
    } 
} 
