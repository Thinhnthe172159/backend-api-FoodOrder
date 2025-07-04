using FoodOrder.Dtos;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;

namespace FoodOrder.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync();
        Task<IEnumerable<MenuItemDto>> SearchMenuItemsAsync(MenuSearchFilter filter);
        Task<MenuItemDto> GetMenuItemByIdAsync(int id);
        Task<MenuItemDto> CreateMenuItemAsync(MenuItemDto dto);
        Task<MenuItemDto> UpdateMenuItemAsync(int id, MenuItemDto dto);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync();
    }
}
