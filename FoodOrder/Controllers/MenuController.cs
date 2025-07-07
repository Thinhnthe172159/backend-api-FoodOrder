using FoodOrder.Dtos;
using FoodOrder.IServices;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMenuService menuService, ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems()
        {
            try
            {
                var menuItems = await _menuService.GetMenuItemsAsync();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menu items");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPost("items/search")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> SearchMenuItems(MenuSearchFilter filter)
        {
            try
            {
                if (filter == null)
                {
                    return BadRequest("Filter cannot be null");
                }

                var menuItems = await _menuService.SearchMenuItemsAsync(filter);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching menu items");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("items/{id:int}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItemById(int id)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemByIdAsync(id);
                if (menuItem == null)
                {
                    return NotFound($"Menu item with ID {id} not found");
                }

                return Ok(menuItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menu item with ID {MenuItemId}", id);
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPost("items")]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem([FromForm] CreateMenuItemDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Menu item data cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdItem = await _menuService.CreateMenuItemAsync(dto);
                return CreatedAtAction(
                    nameof(GetMenuItemById),
                    new { id = createdItem.Id },
                    createdItem);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while creating menu item");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating menu item");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPut("items/{id:int}")]
        public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(int id, [FromForm] UpdateMenuItemDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Menu item data cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedItem = await _menuService.UpdateMenuItemAsync(id, dto);
                return Ok(updatedItem);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while updating menu item with ID {MenuItemId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating menu item with ID {MenuItemId}", id);
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            try
            {
                var result = await _menuService.DeleteMenuItemAsync(id);
                if (!result)
                {
                    return NotFound($"Menu item with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting menu item with ID {MenuItemId}", id);
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _menuService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("items/search")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> SearchMenuItemsByQuery(
            [FromQuery] string? keyword = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            try
            {
                var filter = new MenuSearchFilter
                {
                    Keyword = keyword,
                    CategoryId = categoryId,
                    IsAvailable = isAvailable,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };

                var menuItems = await _menuService.SearchMenuItemsAsync(filter);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching menu items by query");
                return StatusCode(500, "Internal server error occurred");
            }
        }
    }
}
