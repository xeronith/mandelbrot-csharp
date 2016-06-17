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
        private double cr = 0, ci = -.8;
        //private double cr = -.8, ci = .156;
        //private double cr = 0.285, ci = 0.01;
        //private double cr = 0.285, ci = 0;
        public Program()
        {
            Text = "Mandelbrot Set";
            Icon = SystemIcons.Shield;
            MaximizeBox = MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            pictureBox = new PictureBox();
            Controls.Add(pictureBox);

            SetImage();

            pictureBox.MouseDown += delegate
            {

                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Files|*.png";
                saveFileDialog.FileName = "Fractal";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    pictureBox.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            };

            var phase = 0;
            var timer = new Timer { Interval = 1000 / 30 };
            timer.Tick += (s, e) =>
            {
                phase++;

                cr += Math.Sin(phase / 100) / 500;
                ci += Math.Cos(phase / 100) / 500;

                SetImage();
            };
            timer.Start();
        }

        private void SetImage()
        {
            var image = GenerateFractal(size / 2, size / 2, size, 4.0);
            ClientSize = pictureBox.Size = image.Size;
            pictureBox.Image = image;
        }

        private unsafe Bitmap GenerateFractal(int centerX, int centerY, int size, double range)
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

                    double x = cReal, y = cImaginary;

                    int iterations = 0;

                    while (x * x + y * y < 4 && iterations < maxIterations)
                    {
                        var _x = x * x - y * y + cr;
                        y = 2 * x * y + ci;
                        x = _x;

                        iterations++;
                    }

                    int offset = column * bytesPerPixel;

                    if (iterations < maxIterations)
                    {
                        double norm = iterations / (double)maxIterations;
                        int max = 255;

                        //R
                        pixel[offset + 2] = (byte)(norm * max / 4);
                        //G
                        pixel[offset + 1] = (byte)(norm * max / 2);
                        //B
                        pixel[offset + 0] = (byte)(norm * max / 1);
                    }
                    else
                    {
                        pixel[offset + 2] = 64;
                        pixel[offset + 1] = 128;
                        pixel[offset + 0] = 255;
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
