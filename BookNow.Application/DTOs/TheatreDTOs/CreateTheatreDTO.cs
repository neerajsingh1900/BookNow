using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.TheatreDTOs
{
    public class CreateTheatreDTO
    {
        // This will be set by the Controller from the authenticated user context
        public string OwnerId { get; set; } = null!;

        public string TheatreName { get; set; } = null!;
        public int CityId { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
