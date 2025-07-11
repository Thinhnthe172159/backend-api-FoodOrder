using FoodOrder.IServices;
using FoodOrderApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Security.Claims;

namespace FoodOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;

        public OrderController(IOrderService orderService,
                                IOrderItemService orderItemService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        /* ------------------------------------------------------------------
         * 1.  ĐƠN HÀNG (ORDER)
         * ---------------------------------------------------------------- */

        // GET api/orders?customerId=1&status=paid
        [HttpGet]
        public async Task<IActionResult> GetOrders(
                [FromQuery] int? customerId,
                [FromQuery] string? status)
        {
            var list = await _orderService.GetOrdersAsync(customerId, status);
            return Ok(list);
        }

        // GET api/orders/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return order == null ? NotFound() : Ok(order);
        }

        // POST api/orders
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null) return NotFound("Not found user");
            dto.CustomerId = int.Parse(id.ToString());
            var order = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT api/orders/5/status
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            try
            {
                var ok = await _orderService.UpdateOrderStatusAsync(id, status);
                return ok ? Ok("Cập nhật trạng thái thành công") : NotFound("Không tìm thấy order");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // DELETE api/Cancel/orders/5
        [HttpPost("Cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var ok = await _orderService.CancelOrderAsync(id);
                return ok ? Ok($"Đã hủy order {id} ") : NotFound($"Khongo tìm thấy order {id}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Confirm_order")]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var staffId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(staffId))
            {
                var result = await _orderService.ConfirmOrderAsync(id, int.Parse(staffId));
                return result ? Ok("Xác thực thành công") : BadRequest("Xác thực thất bại");
            }
            return BadRequest("Xác thực thất bại");
        }
        /* ------------------------------------------------------------------
         * 2.  CHI TIẾT MÓN (ORDER ITEM)
         * ---------------------------------------------------------------- */

        // GET api/orders/5/items
        [HttpGet("{orderId:int}/items")]
        public async Task<IActionResult> GetItems(int orderId)
        {
            var items = await _orderItemService.GetItemsByOrderIdAsync(orderId);
            return Ok(items);
        }

        // POST api/orders/5/items
        [HttpPost("{orderId:int}/items")]
        public async Task<IActionResult> AddItem(int orderId,
                        [FromBody] OrderItemCreateDto dto)
        {
            dto.OrderId = orderId;
            var item = await _orderItemService.AddItemAsync(dto);
            return Created(string.Empty, item);
        }

        // PUT api/orders/5/items/12/quantity
        [HttpPost("UpdateQuantityOrderItem")]
        public async Task<IActionResult> UpdateQuantity(OrderItemDto dto)
        {
            var ok = await _orderItemService.UpdateQuantityAsync(dto.Id ?? 0, dto.Quantity);
            return ok ? Ok(true) : NotFound(false);
        }

        // DELETE api/orders/5/items/12
        [HttpDelete("{orderId:int}/items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int orderId, int itemId)
        {
            var ok = await _orderItemService.RemoveItemAsync(itemId);
            return ok ? NoContent() : NotFound();
        }

        [HttpPost("SearchOrder")]
        public async Task<IActionResult> FilterOrder(OrderDto dto)
        {
            var list = await _orderService.SearchOrderAsync(dto);
            return Ok(list);
        }

        [Authorize]
        [HttpPost("getMyCurrentOrder")]
        public async Task<IActionResult> GetMyCurrentOrder()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(id))
            {
                var list = await _orderService.GetAllCurrentOrderByCustomer(int.Parse(id));
                return Ok(list);
            }
            return BadRequest("Chưa đăng nhập");
        }
    }
}
