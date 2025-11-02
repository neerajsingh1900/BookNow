using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.BookingDTOs
{
    public class CreateHoldCommandDTO
    {
        public int ShowId { get; set; }

        [Required]
        public List<int> SeatInstanceIds { get; set; } = new List<int>();

        public Dictionary<int, string> SeatVersions { get; set; } = new Dictionary<int, string>();
    }
}
