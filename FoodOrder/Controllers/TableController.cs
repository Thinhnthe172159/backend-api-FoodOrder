using FoodOrder.IServices;
using FoodOrder.Services;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TableController : Controller
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    // POST: api/table
    [HttpPost]
    public async Task<ActionResult<TableDto>> CreateTable([FromBody] TableDto dto)
    {
        var result = await _tableService.CreateTableAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // GET: api/table
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableDto>>> GetAll()
    {
        var tables = await _tableService.GetTablesAsync();
        return Ok(tables);
    }

    // GET: api/table/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TableDto>> GetById(int id)
    {
        var table = await _tableService.GetTableByIdAsync(id);
        if (table == null)
            return NotFound();

        return Ok(table);
    }

    // GET: api/table/qrcode/{code}
    [HttpGet("qrcode/{code}")]
    public async Task<ActionResult<TableDto>> GetByQrCode(string code)
    {
        var table = await _tableService.GetTableByQrCode(code);
        if (table == null)
            return NotFound();

        return Ok(table);
    }

    // PUT: api/table/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] TableStatusUpdateDto dto)
    {
        var success = await _tableService.UpdateTableStatusAsync(id, dto);
        if (!success)
            return NotFound();

        return NoContent();
    }

}
