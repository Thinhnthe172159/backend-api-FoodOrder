using FoodOrder.Models;

namespace FoodOrder.IRepositories
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task DeleteAsync(int id);
        Task UpdateAsync(T entity);
        
    }

    public interface IUserRepository : IAsyncRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }

    public interface ITableRepository : IAsyncRepository<Table>
    {
        Task<IEnumerable<Table>> ListByStatusAsync(string status);
        Task<Table> GetTableByQrCode(string code);
    }

    public interface IMenuItemRepository : IAsyncRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> ListAvailableAsync();
        Task<IEnumerable<MenuItem>> ListByCategoryAsync(int categoryId);
    }

    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<OrderDto>> SearchOrderAsynce(OrderDto orderDto);
        Task<IEnumerable<Order>> ListByCustomerAsync(int customerId);
        Task<IEnumerable<Order>> ListByStatusAsync(string status);

        Task<IEnumerable<OrderDto>> GetAllCurrentOrderByCustomer(int id);
    }

    public interface IPaymentRepository : IAsyncRepository<Payment>
    {
        Task<IEnumerable<Payment>> ListByDateRangeAsync(DateTime from, DateTime to);
    }

    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task<IEnumerable<Category>> SearchByNameAsync(string name);
        Task<Category?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }

    public interface IOrderItemRepository : IAsyncRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> ListByOrderIdAsync(int orderId);
    }
}
