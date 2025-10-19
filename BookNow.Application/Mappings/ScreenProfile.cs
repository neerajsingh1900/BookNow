using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Models;
using System;
using System.Linq; 

namespace BookNow.Application.Mappings
{
    public class ScreenProfile : Profile
    {
        public ScreenProfile()
        {
           CreateMap<Screen, ScreenDetailsDTO>()
              .ForMember(dest => dest.CurrentShowCount, opt => opt.MapFrom(src =>
                    src.Shows != null ? src.Shows.Count(s => s.EndTime > DateTime.UtcNow) : 0));


             CreateMap<ScreenUpsertDTO, Screen>()
                .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.NumberOfRows * src.SeatsPerRow))
                .ForMember(dest => dest.Theatre, opt => opt.Ignore())
                .ForMember(dest => dest.Seats, opt => opt.Ignore())
                .ForMember(dest => dest.Shows, opt => opt.Ignore());

        }
    }
}
