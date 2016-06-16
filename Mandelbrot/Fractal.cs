using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandelbrot
{
    public delegate void ImageHandler(Image fractal);
    public static class Fractal
    {
        public static event ImageHandler ImageGenerated;
        public static unsafe void Generate(int centerX, int centerY, int size, double range)
        {
            var image = new Bitmap(size, size, PixelFormat.Format24bppRgb);
            var bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);

            byte* scan0 = (byte*)bitmapData.Scan0.ToPointer();
            int bytesPerPixel = 3, stride = bitmapData.Stride;

            int width = bitmapData.Width, height = bitmapData.Height, maxIterations = 32;

            for (int row = 0; row < height; row++)
            {
                byte* pixel = scan0 + (row * stride);

                for (int column = 0; column < width; column++)
                {
                    var cReal = (column - centerX) * range / width;
                    var cImaginary = (row - centerY) * range / height;

                    double x = 0, y = 0;

                    int iterations = 0;

                    while (x * x + y * y < 4 && iterations < maxIterations)
                    {
                        var _x = x * x - y * y + cReal;
                        y = 2 * x * y - cImaginary;
                        x = _x;

                        iterations++;
                    }

                    int offset = column * bytesPerPixel;

                    if (iterations < maxIterations)
                    {
                        double norm = iterations / (double)maxIterations;
                        int max = 255;

                        //R
                        pixel[offset + 2] = (byte)(norm * max / 1);
                        //G
                        pixel[offset + 1] = (byte)(norm * max / 2);
                        //B
                        pixel[offset + 0] = (byte)(norm * max / 4);
                    }
                    else
                    {
                        pixel[offset + 2] = 0;
                        pixel[offset + 1] = 0;
                        pixel[offset + 0] = 0;
                    }
                }
            }

            image.UnlockBits(bitmapData);

            ImageGenerated?.Invoke(image);
        }
    }
}
