using GyanGanga.Web.Data; 
using GyanGanga.Web.Services.Interfaces; 
 
namespace GyanGanga.Web.Services 
{ 
    public class BookService : IBookService 
    { 
        private readonly GyanGangaDbContext _context; 
 
        public BookService(GyanGangaDbContext context) 
        { 
            _context = context; 
        } 
    } 
} 
