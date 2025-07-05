using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;

namespace FoodOrder.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserDetailsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> UpdateUserProfileAsync(int id, UserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
