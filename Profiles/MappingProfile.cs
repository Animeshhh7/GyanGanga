using AutoMapper;
using GyanGanga.Web.Models.Entities;
using GyanGanga.Web.Models.ViewModels;

namespace GyanGanga.Web.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookListViewModel>();
            CreateMap<Book, BookDetailsViewModel>();
        }
    }
}