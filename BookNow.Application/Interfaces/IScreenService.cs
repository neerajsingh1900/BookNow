using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    // Contract for screen-related business logic
    public interface IScreenService
    {
     
       // Task<ScreenDetailsDTO?> GetScreenDetailsByIdAsync(int screenId);
            
       
        Task<IEnumerable<ScreenDetailsDTO>> GetScreensByTheatreIdAsync(int theatreId);

        
     //   Task<ScreenDetailsDTO> UpsertScreenAsync(ScreenUpsertDTO dto);

      
    }
}
