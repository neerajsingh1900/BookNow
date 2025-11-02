using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.SearchDTOs
{
    public class ShowtimeDTO
    {
        public int ShowId { get; set; }
        public DateTime StartTime { get; set; } 
        public string ScreenName { get; set; } = null!;
        public bool IsCancellable { get; set; } = false; 
    }
}
