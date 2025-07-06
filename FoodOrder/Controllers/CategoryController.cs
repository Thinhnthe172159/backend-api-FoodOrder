using FoodOrder.Dtos;
using FoodOrder.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid category ID" });

                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new { message = $"Category with ID {id} not found" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm danh mục theo tên
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategories(
            [FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest(new { message = "Search name is required" });

                var categories = await _categoryService.SearchCategoriesAsync(name);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh mục theo tên chính xác
        /// </summary>
        [HttpGet("name/{name}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest(new { message = "Category name is required" });

                var category = await _categoryService.GetCategoryByNameAsync(name);
                if (category == null)
                    return NotFound(new { message = $"Category '{name}' not found" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(
            [FromBody] CreateCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto == null)
                    return BadRequest(new { message = "Category data is required" });

                var category = await _categoryService.CreateCategoryAsync(dto);
                return CreatedAtAction(
                    nameof(GetCategory),
                    new { id = category.Id },
                    category);
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
        /// Cập nhật danh mục
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(
            int id, [FromBody] UpdateCategoryDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid category ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto == null)
                    return BadRequest(new { message = "Category data is required" });

                var category = await _categoryService.UpdateCategoryAsync(id, dto);
                return Ok(category);
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
        /// Xóa danh mục
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid category ID" });

                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound(new { message = $"Category with ID {id} not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra danh mục có tồn tại không
        /// </summary>
        /*[HttpGet("exists/{name}")]
        public async Task<ActionResult<bool>> CategoryExists(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest(new { message = "Category name is required" });

                var exists = await _categoryService.CategoryExistsAsync(name);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }*/
    }
}
