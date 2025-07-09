using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/users/5
        [Authorize]
        [HttpGet("/userProfile")]
        public async Task<ActionResult<UserDto>> GetUserById()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("Lỗi chưa đăng nhập!");
            }
            var user = await _userService.GetUserDetailsByIdAsync(int.Parse(userId));
            if (user == null) return NotFound();
            return Ok(user);
        }

        // PUT: api/users/5
        [Authorize]
        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("Lỗi chưa đăng nhập!");
            }
            var updated = await _userService.UpdateUserProfileAsync(int.Parse(userId), dto);
            return Ok(updated);
        }

        // DELETE: api/users/id
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // GET: api/users/roles
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
        {
            var roles = await _userService.GetAllRolesAsync();
            return Ok(roles);
        }
    }
}

