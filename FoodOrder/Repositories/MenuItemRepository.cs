using FoodOrder.IRepositories;
using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(FoodOrderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MenuItem>> ListAvailableAsync()
        {
            return await _context.Set<MenuItem>()
                .Where(m => m.IsAvailable == true)
                .Include(m => m.Category)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> ListByCategoryAsync(int categoryId)
        {
            return await _context.Set<MenuItem>()
                .Where(m => m.CategoryId == categoryId && m.IsAvailable == true)
                .Include(m => m.Category)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }
    }
}
