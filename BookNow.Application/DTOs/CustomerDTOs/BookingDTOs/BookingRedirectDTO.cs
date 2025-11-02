using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.BookingDTOs
{
    public class BookingRedirectDTO
    {
        public bool Success { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
