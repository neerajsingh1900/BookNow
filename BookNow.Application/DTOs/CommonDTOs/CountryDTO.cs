using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CommonDTOs
{
    public class CountryDTO
    {
        public int CountryId { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
