using FoodOrder.Models;

namespace FoodOrder.IRepositories
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> ListAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }

    public interface IUserRepository : IAsyncRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }

    public interface ITableRepository : IAsyncRepository<Table>
    {
        Task<IEnumerable<Table>> ListByStatusAsync(string status);
        Task<IEnumerable<Table>> GetTableByQrCode(string code);
    }

    public interface IMenuItemRepository : IAsyncRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> ListAvailableAsync();
        Task<IEnumerable<MenuItem>> ListByCategoryAsync(int categoryId);
    }

    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<Order>> ListByCustomerAsync(int customerId);
        Task<IEnumerable<Order>> ListByStatusAsync(string status);
    }

    public interface IPaymentRepository : IAsyncRepository<Payment>
    {
        Task<IEnumerable<Payment>> ListByDateRangeAsync(DateTime from, DateTime to);
    }

    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task<IEnumerable<Category>> SearchByNameAsync(string name);
    }
}
