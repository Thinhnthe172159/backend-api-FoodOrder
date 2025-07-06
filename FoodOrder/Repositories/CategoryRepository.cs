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
                .ToListAsync();
        }
    }
}
