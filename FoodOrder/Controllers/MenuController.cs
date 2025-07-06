using FoodOrder.Dtos;
using FoodOrder.IServices;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Lấy danh sách tất cả món ăn có sẵn
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems()
        {
            try
            {
                var menuItems = await _menuService.GetMenuItemsAsync();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm món ăn theo filter
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> SearchMenuItems(
            [FromBody] MenuSearchFilter filter)
        {
            try
            {
                if (filter == null)
                    return BadRequest(new { message = "Search filter is required" });

                var menuItems = await _menuService.SearchMenuItemsAsync(filter);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin món ăn theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid menu item ID" });

                var menuItem = await _menuService.GetMenuItemByIdAsync(id);
                if (menuItem == null)
                    return NotFound(new { message = $"MenuItem with ID {id} not found" });

                return Ok(menuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo món ăn mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem(
            [FromBody] CreateMenuItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto == null)
                    return BadRequest(new { message = "Menu item data is required" });

                var menuItem = await _menuService.CreateMenuItemAsync(dto);
                return CreatedAtAction(
                    nameof(GetMenuItem),
                    new { id = menuItem.Id },
                    menuItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin món ăn
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(
            int id, [FromBody] UpdateMenuItemDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid menu item ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto == null)
                    return BadRequest(new { message = "Menu item data is required" });

                var menuItem = await _menuService.UpdateMenuItemAsync(id, dto);
                return Ok(menuItem);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa món ăn
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid menu item ID" });

                var result = await _menuService.DeleteMenuItemAsync(id);
                if (!result)
                    return NotFound(new { message = $"MenuItem with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// </summary>
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
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy món ăn theo danh mục (GET alternative cho search)
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItemsByCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    return BadRequest(new { message = "Invalid category ID" });

                var filter = new MenuSearchFilter { CategoryId = categoryId };
                var menuItems = await _menuService.SearchMenuItemsAsync(filter);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm món ăn theo từ khóa (GET alternative)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> SearchMenuItemsByKeyword(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] bool? isAvailable,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
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
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
