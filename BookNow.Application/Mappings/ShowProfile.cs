using AutoMapper;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq;

namespace BookNow.Application.Mappings
{
    public class ShowProfile : Profile
    {
        public ShowProfile() {
       CreateMap<Show, ShowDetailsDTO>()
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
            .ForMember(dest => dest.MovieGenre, opt => opt.MapFrom(src => src.Movie.Genre ?? "N/A"))
            .ForMember(dest => dest.MovieDurationMinutes, opt => opt.MapFrom(src => src.Movie.Duration))
            .ForMember(dest => dest.MoviePosterUrl, opt => opt.MapFrom(src => src.Movie.PosterUrl));


        CreateMap<Screen, ScreenShowListDTO>()
            .ForMember(dest => dest.TheatreName, opt => opt.MapFrom(src => src.Theatre.TheatreName))
           .ForMember(dest => dest.Shows, opt => opt.MapFrom(src =>
                src.Shows.OrderBy(s => s.StartTime)));
    }
      
}
}
