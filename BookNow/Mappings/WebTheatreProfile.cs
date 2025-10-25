using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Screen;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Show;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Theatre;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookNow.Web.Mappings
{
    public class WebTheatreProfile : Profile
    {
        public WebTheatreProfile()
        {
           
            CreateMap<TheatreDetailDTO, TheatreListItemVM>();

          
            CreateMap<TheatreUpsertVM, TheatreUpsertDTO>();

            CreateMap<ScreenUpsertVM, ScreenUpsertDTO>();

            CreateMap<ScreenDetailsDTO, ScreenUpsertVM>();

           

                   }
    }
}
