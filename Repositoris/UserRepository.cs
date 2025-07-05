using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodOrder.Repositoris
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly FoodOrderDbContext _context;
        public UserRepository(FoodOrderDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
