using AutoMapper; 
using GyanGanga.Web.Data; 
using GyanGanga.Web.Models.Entities; 
using GyanGanga.Web.Models.ViewModels; 
using GyanGanga.Web.Services.Interfaces; 
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks; 
 
namespace GyanGanga.Web.Services 
{ 
    public class BookService : IBookService 
    { 
        private readonly GyanGangaDbContext _context; 
        private readonly IMapper _mapper; 
 
        public BookService(GyanGangaDbContext context, IMapper mapper) 
        { 
            _context = context; 
            _mapper = mapper; 
        } 
 
        public async Task<List<BookListViewModel>> GetBooksAsync() 
        { 
            var books = await _context.Books.ToListAsync(); 
            return _mapper.Map<List<BookListViewModel>>(books); 
        } 
 
        public async Task<BookDetailsViewModel?> GetBookByIdAsync(int id) 
        { 
            var book = await _context.Books.FindAsync(id); 
            return book == null ? null : _mapper.Map<BookDetailsViewModel>(book); 
        } 
    } 
} 
