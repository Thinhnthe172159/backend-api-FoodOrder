using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly FoodOrderDbContext _context;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _context = new FoodOrderDbContext();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                await _userRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Role)
                        .Select(u => new UserDto
                        {
                            Id = u.Id,
                            RoleId = u.Id,
                            Username = u.Username,
                            FullName = u.FullName,
                            Phone = u.Phone,
                            Email = u.Email,
                            RoleName = u.Role.Name
                        }).ToListAsync();
        }

        public async Task<UserDto> GetUserDetailsByIdAsync(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name
                };
            }
            return null;
        }

        public async Task<UserDto> UpdateUserProfileAsync(int id, UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                try
                {
                    user.FullName = dto.FullName;
                    user.Phone = dto.Phone;
                    user.Email = dto.Email;
                    await _userRepository.UpdateAsync(user);
                    return dto;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return dto;
        }
    }
}
