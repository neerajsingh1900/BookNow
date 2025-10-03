using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; } // pk, increment

        [Required]
        public string Title { get; set; } = null!; // varchar

        public string? Genre { get; set; } // varchar
        public string? Language { get; set; } // varchar
        public int Duration { get; set; } // int (minutes)
        public DateOnly ReleaseDate { get; set; } // date

        [Url]
        public string? PosterUrl { get; set; } // varchar

        // Foreign Key to Users (ProducerId varchar [ref: > Users.UserId])
        public string ProducerId { get; set; } = null!; // string (to match IdentityUser PK)

        // Navigation property for Producer (Many-to-One)
        [ForeignKey("ProducerId")]
        public virtual ApplicationUser Producer { get; set; } = null!;

        public DateTime CreatedAt { get; set; } // datetime
        public DateTime UpdatedAt { get; set; } // datetime

        // Navigation collections
        public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
