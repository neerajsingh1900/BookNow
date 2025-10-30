using AutoMapper;
using BookNow.Application.DTOs.CommonDTOs;
using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Mappings
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<Country, CountryDTO>();
            CreateMap<City, CityDTO>();
        }
    }
}
