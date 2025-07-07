using FoodOrder.Dtos;
using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodOrder.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtSettings _jwtSettings;
        private readonly PasswordHasher<User> _hasher;

        public AuthService(IUserRepository userRepo, IOptions<JwtSettings> jwtOptions)
        {
            _userRepo = userRepo;
            _jwtSettings = jwtOptions.Value;
            _hasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result != PasswordVerificationResult.Success) return null;

            return new AuthResponseDto
            {
                Token = GenerateToken(user),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.Name
                }
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userRepo.GetByUsernameAsync(dto.Username);
            if (existing != null) throw new Exception("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = _hasher.HashPassword(null, dto.Password),
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                RoleId = dto.RoleId
            };

            await _userRepo.AddAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                RoleId = user.RoleId
            };
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            // Optional: send token to email
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userRepo.GetAllAsync()
                                      .ContinueWith(t => t.Result.FirstOrDefault(u => u.Email == dto.Email));
            if (user == null) return false;

            user.PasswordHash = _hasher.HashPassword(user, dto.NewPassword);
            await _userRepo.UpdateAsync(user);
            return true;
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpireDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

