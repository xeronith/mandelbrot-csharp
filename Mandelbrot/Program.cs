using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mandelbrot
{
    class Program : Form
    {
        private PictureBox pictureBox;
        private int size = 500;
        public Program()
        {
            Text = "Mandelbrot Set";
            Icon = SystemIcons.Shield;
            MaximizeBox = MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            pictureBox = new PictureBox();
            Controls.Add(pictureBox);

            SetImage(size / 2, size / 2);

            pictureBox.MouseDown += delegate {

                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Files|*.png";
                saveFileDialog.FileName = "Fractal";

                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                    pictureBox.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            };
        }

        private void SetImage(int x, int y)
        {
            var image = GenerateFractal(x, y, size, 4.0);
            ClientSize = pictureBox.Size = image.Size;
            pictureBox.Image = image;
        }

        private unsafe Bitmap GenerateFractal(int centerX, int centerY, int size, double range)
        {
            var image = new Bitmap(size, size, PixelFormat.Format24bppRgb);
            var bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);

            byte* scan0 = (byte*)bitmapData.Scan0.ToPointer();
            int bytesPerPixel = 3, stride = bitmapData.Stride;

            int width = bitmapData.Width, height = bitmapData.Height, maxIterations = 64;

            for (int row = 0; row < height; row++)
            {
                byte* pixel = scan0 + (row * stride);

                for (int column = 0; column < width; column++)
                {
                    var cReal = (column - centerX) * range / width;
                    var cImaginary = (row - centerY) * range / height;

                    double x = cReal, y = cImaginary;

                    int iterations = 0;

                    while(x * x + y * y < 4 && iterations < maxIterations)
                    {
                        var _x = x * x - y * y;
                        y = 2 * x * y - .8;
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

            return image;
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new Program());
        }
    }
}
