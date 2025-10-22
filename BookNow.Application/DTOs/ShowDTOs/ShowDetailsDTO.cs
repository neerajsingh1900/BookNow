using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.ShowDTOs
{
    public class ShowDetailsDTO
    {
        public int ShowId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string MovieGenre { get; set; } = string.Empty;
        public int MovieDurationMinutes { get; set; }
        public string? MoviePosterUrl { get; set; }
    }
}
