using FoodOrderApp.Application.DTOs;

namespace FoodOrder.IServices
{
    public interface ITableService
    {
        Task<TableDto> CreateTableAsync(TableDto dto);
        Task<IEnumerable<TableDto>> GetTablesAsync();
        Task<TableDto> GetTableByIdAsync(int id);
        Task<TableDto> GetTableByQrCode(string code);
        Task<bool> UpdateTableStatusAsync(int id, TableStatusUpdateDto dto);
    }
}
