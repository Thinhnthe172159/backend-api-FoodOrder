using FoodOrder.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.Controllers
{
    [Authorize(Roles = "Manager")]
    [ApiController]
    [Route("api/qr")]
    public class QrController : ControllerBase
    {
        private readonly QrCodeCloudService _qr;

        public QrController(QrCodeCloudService qr) => _qr = qr;

        [HttpPost("{number:int}")]
        public async Task<IActionResult> Create(int number)
        {
            string url = await _qr.GenerateAndUploadAsync(number.ToString());
            return Ok(new { url });
        }
    }

}
