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
        public Program()
        {
            Text = "Mandelbrot Set";
            Icon = SystemIcons.Shield;
            MaximizeBox = MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            pictureBox = new PictureBox();
            Controls.Add(pictureBox);
            pictureBox.MouseDown += delegate {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Files|*.png";
                saveFileDialog.FileName = "Fractal";

                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                    pictureBox.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            };

            Fractal.ImageGenerated += fractal => {
                ClientSize = pictureBox.Size = fractal.Size;
                pictureBox.Image = fractal;
            };

            Fractal.Generate(250, 250, 500, 4.0);
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new Program());
        }
    }
}
