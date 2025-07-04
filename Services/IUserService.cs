using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;

namespace FoodOrder.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserDetailsByIdAsync(int id);
        Task<UserDto> UpdateUserProfileAsync(int id, UserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
