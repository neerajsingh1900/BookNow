using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace BookNow.Application.DTOs.ShowDTOs
{
    public class ScreenMetadataDTO
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public int ScreenId { get; set; }
        public string ScreenNumber { get; set; } = string.Empty;
    }
}
