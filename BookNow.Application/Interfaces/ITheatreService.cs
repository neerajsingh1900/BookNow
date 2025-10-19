using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface ITheatreService
    {
        Task<TheatreDetailDTO> AddTheatreAsync(string ownerId, TheatreUpsertDTO dto);
        Task<IEnumerable<TheatreDetailDTO>> GetOwnerTheatresAsync(string ownerId);

        Task<int> AddScreenAndSeatsAsync(int theatreId, ScreenUpsertDTO dto);
        Task<IEnumerable<Screen>> GetTheatreScreensAsync(int theatreId);

        Task<Show> AddShowAsync(ShowCreationDTO dto);
        Task<IEnumerable<Show>> GetScreenShowsAsync(int screenId);
        Task<bool> IsOwnerOfTheatreAsync(string userId, int theatreId);

        Task<TheatreDetailDTO> GetTheatreByIdAsync(int theatreId, string ownerId);
        Task<TheatreDetailDTO> UpdateTheatreAsync(int theatreId, TheatreUpsertDTO dto, string ownerId);

    }
}