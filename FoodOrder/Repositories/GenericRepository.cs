using FoodOrder.IRepositories;
using FoodOrder.Models;
using FoodOrder.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repositories
{
    public class GenericRepository<TContext, T> : IAsyncRepository<T>
      where T : class
      where TContext : DbContext
    {
        protected readonly TContext _context;

        public GenericRepository(TContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
    public class GenericRepository<T> : GenericRepository<FoodOrderDbContext, T>, IAsyncRepository<T>
        where T : class
    {
        public GenericRepository(FoodOrderDbContext context) : base(context) { }
    }
}

