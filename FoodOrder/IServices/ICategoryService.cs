using FoodOrder.Dtos;
using FoodOrder.Models;

namespace FoodOrder.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto?> GetCategoryByNameAsync(string name);
        Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string name);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(string name);
    }
}
