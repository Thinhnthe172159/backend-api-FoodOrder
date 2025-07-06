using FoodOrder.IServices;
using FoodOrder.IRepositories;
using FoodOrder.Dtos;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly FoodOrderDbContext _context;

        public MenuService(IMenuItemRepository menuItemRepository, ICategoryRepository categoryRepository, FoodOrderDbContext foodOrderDbContext)
        {
            _menuItemRepository = menuItemRepository;
            _categoryRepository = categoryRepository;
            _context = foodOrderDbContext;
        }

        public async Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync()
        {
            var menuItems = await _menuItemRepository.ListAvailableAsync();
            return menuItems.Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                ImageUrl = m.ImageUrl,
                IsAvailable = m.IsAvailable,
                CategoryName = m.Category?.Name,
                CategoryId = m.CategoryId
            });
        }

        public async Task<IEnumerable<MenuItemDto>> SearchMenuItemsAsync(MenuSearchFilter filter)
        {
            var filteredItems = _context.MenuItems.Include(m => m.Category).AsQueryable();


            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                filteredItems = filteredItems.Where(m => m.Name.ToLower().Contains(filter.Keyword.ToLower()) || m.Description.ToLower().Contains(filter.Keyword.ToLower()));
            }

            if (filter.CategoryId.HasValue)
            {
                filteredItems = filteredItems.Where(m => m.CategoryId == filter.CategoryId);
            }

            if (filter.IsAvailable.HasValue)
            {
                filteredItems = filteredItems.Where(m => m.IsAvailable == filter.IsAvailable);
            }

            if (filter.MinPrice.HasValue)
            {
                filteredItems = filteredItems.Where(m => m.Price >= filter.MinPrice);
            }

            if (filter.MaxPrice.HasValue)
            {
                filteredItems = filteredItems.Where(m => m.Price <= filter.MaxPrice);
            }

            return await filteredItems.Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                ImageUrl = m.ImageUrl,
                IsAvailable = m.IsAvailable,
                CategoryName = m.Category.Name,
                CategoryId = m.CategoryId
            }).ToListAsync();
        }

        public async Task<MenuItemDto?> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
                return null;

            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
                IsAvailable = menuItem.IsAvailable,
                CategoryName = menuItem.Category?.Name,
                CategoryId = menuItem.CategoryId
            };
        }

        public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto)
        {
            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException($"Category with ID {dto.CategoryId} not found");
            }

            var menuItem = new MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                IsAvailable = dto.IsAvailable,
                CategoryId = dto.CategoryId
            };

            await _menuItemRepository.AddAsync(menuItem);
            var createdItem = await _menuItemRepository.GetByIdAsync(menuItem.Id);

            return new MenuItemDto
            {
                Id = createdItem!.Id,
                Name = createdItem.Name,
                Description = createdItem.Description,
                Price = createdItem.Price,
                ImageUrl = createdItem.ImageUrl,
                IsAvailable = createdItem.IsAvailable,
                CategoryName = createdItem.Category?.Name,
                CategoryId = createdItem.CategoryId
            };
        }

        public async Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
        {
            var existingItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingItem == null)
                throw new ArgumentException($"MenuItem with ID {id} not found");

            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException($"Category with ID {dto.CategoryId} not found");
            }

            existingItem.Name = dto.Name;
            existingItem.Description = dto.Description;
            existingItem.Price = dto.Price;
            existingItem.ImageUrl = dto.ImageUrl;
            existingItem.IsAvailable = dto.IsAvailable;
            existingItem.CategoryId = dto.CategoryId;

            await _menuItemRepository.UpdateAsync(existingItem);

            var updatedItem = await _menuItemRepository.GetByIdAsync(id);

            return new MenuItemDto
            {
                Id = updatedItem!.Id,
                Name = updatedItem.Name,
                Description = updatedItem.Description,
                Price = updatedItem.Price,
                ImageUrl = updatedItem.ImageUrl,
                IsAvailable = updatedItem.IsAvailable,
                CategoryName = updatedItem.Category?.Name,
                CategoryId = updatedItem.CategoryId
            };
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
                return false;

            await _menuItemRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }
    }
}