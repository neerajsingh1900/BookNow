using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;

namespace BookNow.Areas.Customer.ViewModels
{
    public class MovieListingVM
    {
        public List<(int Id, string Name)> Countries { get; set; } = new();
        public List<(int Id, string Name)> Cities { get; set; } = new();
        public int? SelectedCountryId { get; set; }
        public int? SelectedCityId { get; set; }
        public List<MovieListingDTO> Movies { get; set; } = new();
    }
}
