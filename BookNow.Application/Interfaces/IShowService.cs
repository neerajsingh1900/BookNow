using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IShowService
    {

        Task<ScreenMetadataDTO> GetScreenMetadataAsync(int screenId);
        Task<IEnumerable<ShowDetailsDTO>> GetShowsForScreenAsync(int screenId);

        Task<Show> AddShowAsync(ShowCreationDTO dto);
        Task<ShowMovieListDTO> GetShowCreationDataAsync(int screenId);
    }
}
