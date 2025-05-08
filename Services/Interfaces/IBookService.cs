using GyanGanga.Web.Models.ViewModels; 
using System.Threading.Tasks; 
 
namespace GyanGanga.Web.Services.Interfaces 
{ 
    public interface IBookService 
    { 
        Task<List<BookListViewModel>> GetBooksAsync(); 
        Task<BookDetailsViewModel?> GetBookByIdAsync(int id); 
    } 
} 
