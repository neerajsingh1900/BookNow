namespace BookNow.Application.DTOs.TheatreDTOs
{
    
    public class TheatreUpsertDTO
    {
        public int? TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}