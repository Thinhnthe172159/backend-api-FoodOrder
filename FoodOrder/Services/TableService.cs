using CloudinaryDotNet.Actions;
using FoodOrder.Extensions;
using FoodOrder.IRepositories;
using FoodOrder.IServices;
using FoodOrder.Models;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Services;

public class TableService : ITableService
{
    private readonly FoodOrderDbContext _context;
    private readonly ITableRepository tableRepository;
    private readonly QrCodeCloudService qrCodeCloudService;
    public TableService(FoodOrderDbContext context,
        ITableRepository tableRepository,
        QrCodeCloudService qrCodeCloudService)
    {
        _context = context;
        this.tableRepository = tableRepository;
        this.qrCodeCloudService = qrCodeCloudService;
    }

    public async Task<TableDto?> CreateTableAsync(TableDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TableNumber))
            return null;

        // Kiểm tra bàn đã tồn tại
        var exists = await _context.Tables
            .AnyAsync(t => t.TableNumber == dto.TableNumber);

        if (exists)
            throw new Exception("TableNumber already exists");

        var table = new Table
        {
            TableNumber = dto.TableNumber.Trim(),
            Qrcode = "QRCODE",
            Status = "available"
        };

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        var newestTable = await _context.Tables.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
        if (newestTable != null)
        {
            var qrCode = await qrCodeCloudService.GenerateAndUploadAsync(newestTable.Id.ToString());

            newestTable.Qrcode = qrCode;
            await tableRepository.UpdateAsync(newestTable);

            dto.Id = table.Id;
            dto.QRCode = table.Qrcode;
            return dto;
        }
        return null;
    }

    public async Task<IEnumerable<TableDto>> GetTablesAsync()
    {
        var listTable = await _context.Tables
            .Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                QRCode = t.Qrcode,
                Status = t.Status
            })
            .ToListAsync();

        return listTable;
    }

    public async Task<TableDto?> GetTableByIdAsync(int id)
    {
        if (id <= 0) return null;

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

    public async Task<TableDto?> GetTableByQrCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;

        var table = await tableRepository.GetTableByQrCode(code);
        if (table == null) return null;

        return new TableDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            QRCode = table.Qrcode,
            Status = table.Status
        };
    }

    public async Task<bool> UpdateTableStatusAsync(int id, TableStatusUpdateDto dto)
    {
        if (id <= 0 || dto.Status == null) return false;

        var table = await _context.Tables.FindAsync(id);
        if (table == null) return false;

        table.Status = dto.Status.Trim();
        await _context.SaveChangesAsync();
        return true;
    }
}
