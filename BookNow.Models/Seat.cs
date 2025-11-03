using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; } 
        public int ScreenId { get; set; }

        [ForeignKey("ScreenId")]
        public virtual Screen Screen { get; set; } = null!;

      public string SeatNumber { get; set; } = null!;

        public string? RowLabel { get; set; } 
        public int SeatIndex { get; set; } 

        public virtual ICollection<SeatInstance> SeatInstances { get; set; } = new List<SeatInstance>();
    }
}
