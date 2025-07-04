using FoodOrderApp.Application.DTOs;

namespace FoodOrder.Services
{
    public interface ITableService
    {
        Task<TableDto> CreateTableAsync(TableDto dto);
        Task<IEnumerable<TableDto>> GetTablesAsync();
        Task<TableDto> GetTableByIdAsync(int id);
        Task<bool> UpdateTableStatusAsync(int id, TableStatusUpdateDto dto);
    }
}
