using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Services;

public class TableService : ITableService
{
    private readonly FoodOrderDbContext _context;
    private readonly ITableRepository tableRepository;
    public TableService(FoodOrderDbContext context, ITableRepository tableRepository)
    {
        _context = context;
        this.tableRepository = tableRepository;
    }

    public async Task<TableDto> CreateTableAsync(TableDto dto)
    {
        var table = new Table
        {
            TableNumber = dto.TableNumber,
            Qrcode = dto.QRCode,
            Status = dto.Status
        };

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        dto.Id = table.Id;
        return dto;
    }

    public async Task<IEnumerable<TableDto>> GetTablesAsync()
    {
        return await _context.Tables
            .Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                QRCode = t.Qrcode,
                Status = t.Status
            })
            .ToListAsync();
    }

    public async Task<TableDto> GetTableByIdAsync(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return null;

        return new TableDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            QRCode = table.Qrcode,
            Status = table.Status
        };
    }

    public async Task<TableDto> GetTableByQrCode(string code)
    {
        var table = tableRepository.GetTableByQrCode(code);

        if (table == null) return null;

        return new TableDto
        {
            Id = table.Result.Id,
            TableNumber = table.Result.TableNumber,
            QRCode = table.Result.Qrcode,
            Status = table.Result.Status
        };
    }

    public async Task<bool> UpdateTableStatusAsync(int id, TableStatusUpdateDto dto)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return false;

        table.Status = dto.Status;
        await _context.SaveChangesAsync();
        return true;
    }
}
