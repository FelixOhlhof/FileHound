using System.Drawing;
using System.Drawing.Imaging;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Graphics.Colors;

namespace PdfSearchWPF.Exceptions
{
  public static class PdfImageExtensions
  {
    public static Bitmap? RenderToBitmap(this IPdfImage image)
    {
      if (!image.TryGetBytesAsMemory(out var memory))
        return null;

      int width = image.WidthInSamples;
      int height = image.HeightInSamples;
      int bitsPerComponent = image.BitsPerComponent;
      int components = GetComponentCountFromColorSpace(image);

      if (bitsPerComponent != 8)
        throw new NotSupportedException($"Only 8-bit components supported. Found: {bitsPerComponent}");

      int expectedLength = width * height * components;
      if (memory.Length < expectedLength)
        return null;

      var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
      var span = memory.Span;

      int i = 0;
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          Color color = components switch
          {
            1 => Color.FromArgb(span[i], span[i], span[i++]),

            3 => Color.FromArgb(span[i++], span[i++], span[i++]),

            4 => // Approximate CMYK to RGB
                ConvertCmykToRgb(span[i++], span[i++], span[i++], span[i++]),

            _ => Color.Black
          };

          bitmap.SetPixel(x, y, color);
        }
      }

      return bitmap;
    }

    private static int GetComponentCountFromColorSpace(IPdfImage image)
    {
      if (image.IsImageMask)
        return 1;

      var type = image.ColorSpaceDetails?.Type ?? ColorSpace.DeviceGray;

      return type switch
      {
        ColorSpace.DeviceGray => 1,
        ColorSpace.DeviceRGB => 3,
        ColorSpace.DeviceCMYK => 4,
        _ => 1 // fallback für nicht unterstützte Farbräume
      };
    }

    private static Color ConvertCmykToRgb(byte c, byte m, byte y, byte k)
    {
      // CMYK to RGB approximation (naive)
      int r = 255 - Math.Min(255, c + k);
      int g = 255 - Math.Min(255, m + k);
      int b = 255 - Math.Min(255, y + k);
      return Color.FromArgb(r, g, b);
    }
  }
}
