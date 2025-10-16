using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Theatre;

namespace BookNow.Web.Mappings
{
    public class WebTheatreProfile : Profile
    {
        public WebTheatreProfile()
        {
            // DTO -> ViewModel (for displaying in views)
            CreateMap<TheatreDetailDTO, TheatreListItemVM>();

            // Upsert mapping: ViewModel -> DTO (for POST/PUT)
            CreateMap<TheatreUpsertVM, TheatreUpsertDTO>();
        }
    }
}
