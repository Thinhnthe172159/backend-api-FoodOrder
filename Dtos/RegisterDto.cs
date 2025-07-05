using System.ComponentModel.DataAnnotations;

namespace FoodOrderApp.Application.DTOs;

public class RegisterDto
{
    [Required]
    public string Username { get; set; }
    [Required, MinLength(6)]
    public string Password { get; set; }
    [Required]
    [Compare("Password", ErrorMessage = "Password và ConfirmPassword không khớp")]
    public string ConfirmPassword { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
}
