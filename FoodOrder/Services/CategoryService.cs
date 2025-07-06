using FoodOrder.Dtos;
using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;

namespace FoodOrder.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMenuItemRepository _menuItemRepository;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMenuItemRepository menuItemRepository)
        {
            _categoryRepository = categoryRepository;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var result = new List<CategoryDto>();

            foreach (var category in categories)
            {
                var menuItems = await _menuItemRepository.ListByCategoryAsync(category.Id);
                result.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    MenuItemCount = menuItems.Count()
                });
            }

            return result;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            var menuItems = await _menuItemRepository.ListByCategoryAsync(category.Id);
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                MenuItemCount = menuItems.Count()
            };
        }

        public async Task<CategoryDto?> GetCategoryByNameAsync(string name)
        {
            var category = await _categoryRepository.GetByNameAsync(name);
            if (category == null)
                return null;

            var menuItems = await _menuItemRepository.ListByCategoryAsync(category.Id);
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                MenuItemCount = menuItems.Count()
            };
        }

        public async Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string name)
        {
            var categories = await _categoryRepository.SearchByNameAsync(name);
            var result = new List<CategoryDto>();

            foreach (var category in categories)
            {
                var menuItems = await _menuItemRepository.ListByCategoryAsync(category.Id);
                result.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    MenuItemCount = menuItems.Count()
                });
            }

            return result;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            // Kiểm tra trùng tên
            var existingCategory = await _categoryRepository.GetByNameAsync(dto.Name);
            if (existingCategory != null)
                throw new ArgumentException($"Danh mục '{dto.Name}' đã tồn tại");

            var category = new Category
            {
                Name = dto.Name
            };

            await _categoryRepository.AddAsync(category);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                MenuItemCount = 0
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new ArgumentException($"Danh mục với ID {id} không tồn tại");

            // Kiểm tra trùng tên (trừ chính nó)
            var duplicateCategory = await _categoryRepository.GetByNameAsync(dto.Name);
            if (duplicateCategory != null && duplicateCategory.Id != id)
                throw new ArgumentException($"Danh mục '{dto.Name}' đã tồn tại");

            existingCategory.Name = dto.Name;
            await _categoryRepository.UpdateAsync(existingCategory);

            var menuItems = await _menuItemRepository.ListByCategoryAsync(id);
            return new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                MenuItemCount = menuItems.Count()
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            // Kiểm tra xem có menu items nào đang sử dụng category này không
            var menuItems = await _menuItemRepository.ListByCategoryAsync(id);
            if (menuItems.Any())
                throw new InvalidOperationException($"Không thể xóa danh mục '{category.Name}' vì vẫn còn {menuItems.Count()} món ăn thuộc danh mục này");

            await _categoryRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> CategoryExistsAsync(string name)
        {
            return await _categoryRepository.ExistsByNameAsync(name);
        }
    }
}
