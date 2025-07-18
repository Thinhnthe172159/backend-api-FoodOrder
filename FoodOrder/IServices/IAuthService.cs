﻿using FoodOrderApp.Application.DTOs;

namespace FoodOrder.IServices
{
    public interface IAuthService
    {

        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
