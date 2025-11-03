using System.ComponentModel.DataAnnotations;

namespace BookNow.Application.DTOs.ScreenDTOs
{
    public class ScreenUpsertDTO
    {
        public int? ScreenId { get; set; }
        public int TheatreId { get; set; }
        public string ScreenNumber { get; set; } = null!;
        public int NumberOfRows { get; set; }
        public int SeatsPerRow { get; set; }
        public decimal DefaultSeatPrice { get; set; }
    }
}   