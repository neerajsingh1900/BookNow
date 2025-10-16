//using AutoMapper;
//using BookNow.Application.DTOs.TheatreDTOs;
//using BookNow.Models;

//namespace BookNow.Application.Mappings
//{
//    public class TheatreProfile : Profile
//    {
//        public TheatreProfile()
//        {
//        //    CreateMap<TheatreUpsertDTO, Theatre>();
//        //    CreateMap<Theatre, TheatreDetailDTO>();
//        CreateMap<TheatreUpsertDTO, Theatre>()
//    .ForMember(dest => dest.TheatreId, opt => opt.Ignore()) // EF generates PK
//    .ForMember(dest => dest.OwnerId, opt => opt.Ignore())   // Set manually in service
//    .ForMember(dest => dest.Screens, opt => opt.Ignore()); // Navigation, not from DTO

//        CreateMap<Theatre, TheatreDetailDTO>()
//    .ForMember(dest => dest.CityName, opt => opt.Ignore())
//    .ForMember(dest => dest.CountryName, opt => opt.Ignore());

//    }
//}
//}

using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Models;

namespace BookNow.Application.Mappings
{
    public class TheatreProfile : Profile
    {
        public TheatreProfile()
        {
            // Mapping from DTO to Entity
            CreateMap<TheatreUpsertDTO, Theatre>()
                .ForMember(dest => dest.TheatreId, opt => opt.Ignore()) // EF generates PK
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())   // Set manually in service
                .ForMember(dest => dest.Screens, opt => opt.Ignore()); // Navigation, not from DTO

            // Mapping from Entity to Detail DTO
            CreateMap<Theatre, TheatreDetailDTO>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City.Country.Name))
                .ForMember(dest => dest.ScreenCount, opt => opt.MapFrom(src => src.Screens.Count));
        }
    }
}
