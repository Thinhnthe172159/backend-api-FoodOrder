using FoodOrder.IRepositories;
using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FoodOrderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> SearchByNameAsync(string name)
        {
            return await _context.Set<Category>()
                .Where(c => c.Name.Contains(name))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Set<Category>()
                .FirstOrDefaultAsync(c => c.Name.Equals(name));
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Set<Category>()
                .AnyAsync(c => c.Name.Equals(name));
        }
    }
}
