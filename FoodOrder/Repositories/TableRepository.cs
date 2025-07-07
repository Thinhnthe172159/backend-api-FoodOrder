using FoodOrder.IRepositories;
using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Repositories
{
    public class TableRepository : GenericRepository<Table>, ITableRepository
    {
        public TableRepository(FoodOrderDbContext context) : base(context)
        {
        }


        public async Task<Table> GetTableByQrCode(string code)
        {
            return await _context.Tables.FirstOrDefaultAsync(o => o.Id == int.Parse(code));
        }

        public async Task<IEnumerable<Table>> ListByStatusAsync(string status)
        {
            return await _context.Tables
                .Where(o => o.Status == status)
                .ToListAsync();
        }
    }
}
