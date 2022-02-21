using System;
using System.Drawing;
using System.Windows.Forms;



namespace voronoi
{
    public partial class FrmFps : Form
    {
        byte[] bytes;
        int t = 0;
        int w = 160;
        int h = 120;

        //----------------------------------
        public FrmFps()
        {
            InitializeComponent();
            w = (this.Width - 10) / 4 * 4;
            h = this.Height - 50;
            bytes = new byte[w * h * 3];
            PicImage.Image = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            timer1.Enabled = true;
        }

        //----------------------------------

        public void refreshimage(Byte[] binImg)
        {
            Bitmap bmp = (Bitmap)PicImage.Image;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            bmpData.Stride = bmp.Width * 3;
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            System.Runtime.InteropServices.Marshal.Copy(binImg, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            PicImage.Image = bmp;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (PicImage.Image != null)
            {
                int j = 0;
                double s = (1.0 + Math.Sin(t / 10.0) * 0.9);
                double theta = t / 60.0;
                int cs = System.Convert.ToInt32(Math.Round(Math.Cos(theta) * s * 1024));
                int ss = System.Convert.ToInt32(Math.Round(Math.Sin(theta) * s * 1024));
                byte a = 0;
                for (int y = -h / 2; y < h / 2; y++)
                {
                    int r1 = -ss * y;
                    int r2 = cs * y;
                    int xx = cs * (-w / 2) + r1;
                    int yy = ss * (-w / 2) + r2;
                    for (int x = -w / 2; x < w / 2; x++)
                    {
                        xx += cs;
                        yy += ss;
                        a = Convert.ToByte((xx >> 10 & 255) ^ (yy >> 10 & 255));
                        bytes[j++] = a;
                        bytes[j++] = a;
                        bytes[j++] = a;
                    }
                }
                timer1.Enabled = true;
                refreshimage(bytes);              
                t++;
            }
        }

        private void FrmVoronoi_Resize(object sender, EventArgs e)
        {   
            timer1.Enabled = false;
            w = (this.ClientRectangle.Width) / 4 * 4;
            h = this.ClientRectangle.Height;
            bytes = new byte[w * h * 3];
            PicImage.Image = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            PicImage.Width= w;
            PicImage.Height = h;
            PicImage.Refresh();
            timer1.Enabled = true;               
        }
    }
}
