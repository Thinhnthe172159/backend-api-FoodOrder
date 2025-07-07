using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrder.Services;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FoodOrder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Debug.WriteLine($"[JWT] userId={id}, username={username}, role={role}");

            if (username == null)
            {
                return Unauthorized();
            }
            return Ok(new
            {
                id = id,
                Username = username,
                Role = role
            });
        }


        /// <summary>
        /// Đăng nhập và nhận JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);
            if (result == null) return Unauthorized("Invalid credentials");

            return Ok(result);
        }

        /// <summary>
        /// Đăng ký tài khoản mới.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var user = await _authService.RegisterAsync(dto);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gửi yêu cầu reset mật khẩu (gửi email chứa token).
        /// </summary>
        [HttpPost("reset-password/request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            var sent = await _authService.RequestPasswordResetAsync(email);
            return sent ? Ok() : NotFound();
        }

        /// <summary>
        /// Xác nhận reset mật khẩu.
        /// </summary>
        [HttpPost("reset-password/confirm")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var ok = await _authService.ResetPasswordAsync(dto);
            return ok ? Ok() : BadRequest();
        }
    }
}
