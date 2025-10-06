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
        public int SeatId { get; set; } // pk, increment

        // Foreign Key to Screens (ref: > Screens.ScreenId)
        public int ScreenId { get; set; }

        // Navigation property for Screen (Many-to-One)
        [ForeignKey("ScreenId")]
        public virtual Screen Screen { get; set; } = null!;

      public string SeatNumber { get; set; } = null!; // varchar

        public string? RowLabel { get; set; } // varchar
        public int SeatIndex { get; set; } // int (physical index for sorting)

        // Navigation collections
        public virtual ICollection<SeatInstance> SeatInstances { get; set; } = new List<SeatInstance>();
    }
}
