using BookNow.Application.DTOs.ShowDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IShowService
    {
        
        Task<ScreenShowListDTO> GetScreenShowsByOwnerAsync(int screenId, string ownerId);
    }
}
