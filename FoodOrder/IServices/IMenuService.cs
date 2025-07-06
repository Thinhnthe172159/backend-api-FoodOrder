using FoodOrder.Dtos;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;

namespace FoodOrder.IServices
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync();
        Task<IEnumerable<MenuItemDto>> SearchMenuItemsAsync(MenuSearchFilter filter);
        Task<MenuItemDto?> GetMenuItemByIdAsync(int id);
        Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto);
        Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    }
}
