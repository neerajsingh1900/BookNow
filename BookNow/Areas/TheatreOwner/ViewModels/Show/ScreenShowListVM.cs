using BookNow.Application.DTOs.ShowDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookNow.Web.Areas.TheatreOwner.ViewModels.Show
{
    public class ScreenShowListVM
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public int ScreenId { get; set; }
        public string ScreenNumber { get; set; } = string.Empty;
        public IEnumerable<ShowDetailsDTO> Shows { get; set; } = new List<ShowDetailsDTO>();
    }
}
