using AutoMapper;
using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Mappings
{
    public class ShowSearchProfile : Profile
    {
        public ShowSearchProfile()
        {
            CreateMap<Movie, MovieListingDTO>()
     .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre ?? ""))
     .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language ?? ""))
     .ForMember(dest => dest.PosterUrl, opt => opt.MapFrom(src => src.PosterUrl ?? "/images/default-poster.png"))
     .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate));

            CreateMap<Show, ShowtimeDTO>()
             .ForMember(dest => dest.ScreenName,
                           opt => opt.MapFrom(src => src.Screen != null ? src.Screen.ScreenNumber : "N/A"))
             .ForMember(dest => dest.IsCancellable,
                           opt => opt.MapFrom(src => src.StartTime > DateTime.Now.AddHours(4)));

        }
    }
}
