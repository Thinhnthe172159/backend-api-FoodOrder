using System.Text;

namespace FoodOrder.Extentions
{
    public static class PayOSHelper
    {
        public static string GenerateSignature(string rawData, string checksumKey)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

}
