using FoodOrder.Extentions;
using FoodOrder.IServices;
using FoodOrder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FoodOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<PayOSOptions> _payOSOptions;
        private readonly IOrderService _orderService;

        public PaymentController(IHttpClientFactory httpClientFactory, IOptions<PayOSOptions> payOSOptions, IOrderService orderService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _payOSOptions = payOSOptions;
            _orderService = orderService;
        }

        [HttpPost("payos")]
        public async Task<IActionResult> CreatePayOSOrder([FromQuery] int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return BadRequest("Không có đơn hàng");
            if (order.TotalAmount == 0) return BadRequest("Không có món ăn được dặt");
            var options = _payOSOptions.Value;

            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long amount = (long)order.TotalAmount;

            var payload = new
            {
                orderCode = orderCode,
                amount = amount,
                description = $"Thanh toán đơn #{orderId}",
                returnUrl = $"http://thinhnguyen5k-001-site1.ltempurl.com/payment-return.html?status=PAID",
                cancelUrl = $"http://thinhnguyen5k-001-site1.ltempurl.com/payment-return.html?status=CANCELLED"
            };

            // Chuỗi rawData để tạo signature: sắp xếp key theo thứ tự alpha
            string rawData = $"amount={amount}&cancelUrl={payload.cancelUrl}&description={payload.description}&orderCode={orderCode}&returnUrl={payload.returnUrl}";
            string signature = PayOSHelper.GenerateSignature(rawData, options.ChecksumKey);

            var fullPayload = new
            {
                orderCode = orderCode,
                amount = amount,
                description = payload.description,
                returnUrl = payload.returnUrl,
                cancelUrl = payload.cancelUrl,
                signature = signature
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-merchant.payos.vn/v2/payment-requests");
            request.Headers.Add("x-client-id", options.ClientId);
            request.Headers.Add("x-api-key", options.ApiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(fullPayload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var resultContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, resultContent);
                }

                dynamic result = JsonConvert.DeserializeObject(resultContent);
                string checkoutUrl = result.data.checkoutUrl;

                return Ok(new { checkoutUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("payos-webhook")]
        public IActionResult Webhook([FromBody] dynamic data)
        {
            Console.WriteLine("Webhook từ PayOS: " + data.ToString());
            return Ok();
        }

        [HttpGet("payment-return")]
        public IActionResult PaymentReturn(string status, string orderCode, string id, bool cancel = false, string code = null)
        {
            if (status == "PAID")
            {
                // Xử lý thành công (cập nhật trạng thái đơn hàng, hiển thị trang thành công...)
                return Content("Thanh toán thành công! Cảm ơn bạn.");
            }
            else if (status == "CANCELLED" || cancel)
            {
                // Xử lý hủy thanh toán (hiển thị thông báo hủy)
                return Content("Bạn đã hủy thanh toán.");
            }
            else
            {
                return Content("Trạng thái thanh toán không xác định.");
            }
        }


    }

}
