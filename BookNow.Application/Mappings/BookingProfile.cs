using AutoMapper;
using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Models;
using System;
using System.Linq;

namespace BookNow.Application.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<SeatInstance, SeatStatusDTO>()
                .ForMember(dest => dest.SeatInstanceId, opt => opt.MapFrom(src => src.SeatInstanceId))
                .ForMember(dest => dest.SeatId, opt => opt.MapFrom(src => src.SeatId))
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.Seat.SeatNumber))
                .ForMember(dest => dest.RowLabel, opt => opt.MapFrom(src => src.Seat.RowLabel))
                .ForMember(dest => dest.SeatIndex, opt => opt.MapFrom(src => src.Seat.SeatIndex))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Seat.Screen.DefaultSeatPrice))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));
        }
    }
}
