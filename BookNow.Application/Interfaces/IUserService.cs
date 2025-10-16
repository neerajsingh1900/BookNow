using BookNow.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IUserService
    {
        Task<IdentityUser?> GetUserByIdAsync(string userId);
        Task<bool> IsUserInRoleAsync(string userId, string role);
    }
}
