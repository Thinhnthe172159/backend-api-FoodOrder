using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using FoodOrder.Extentions;

namespace FoodOrder.Extensions;   

public class QrCodeCloudService
{
    private readonly CloudinaryService _cloud;
    private readonly BarcodeWriterPixelData _writer;

    public QrCodeCloudService(CloudinaryService cloud)
    {
        _cloud = cloud;

        _writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 400,
                Width = 400,
                Margin = 2,
                ErrorCorrection = ErrorCorrectionLevel.Q    
            }
        };
    }


    public async Task<string> GenerateAndUploadAsync(string context)
    {
      
        var pixel = _writer.Write(context);

      
        using var bmp = new Bitmap(pixel.Width, pixel.Height, PixelFormat.Format32bppRgb);
        var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                ImageLockMode.WriteOnly, bmp.PixelFormat);
        try { Marshal.Copy(pixel.Pixels, 0, data.Scan0, pixel.Pixels.Length); }
        finally { bmp.UnlockBits(data); }
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        var bytes = ms.ToArray();

        // 4. Gọi service có sẵn để upload
        string url = await _cloud.UploadImageAsync(bytes, $"qr_{context}.png");
        return url;
    }
}
