using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;


namespace voronoi
{
    public partial class FrmVoronoi : Form
    {

        List<VoronoiItem> voronois = new List<VoronoiItem>();
     
        Bitmap InputBitmap = (Bitmap)Bitmap.FromFile("NINJA-300-3.bmp");
        Byte[,,] pixels =new Byte[640, 480,3];      

        //----------------------------------
        public FrmVoronoi()
        {
            InitializeComponent();
            PicImage.Image = InputBitmap;
            for (int y = 0; y < InputBitmap.Height; y++)
                for (int x = 0; x < InputBitmap.Width; x++)
                {
                    pixels[x, y, 0] = InputBitmap.GetPixel(x, y).R;
                    pixels[x, y, 1] = InputBitmap.GetPixel(x, y).G;
                    pixels[x, y, 2] = InputBitmap.GetPixel(x, y).B;
                }
        }

        //----------------------------------

        private void button1_Click(object sender, EventArgs e)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Random rnd = new Random();
            byte[] bytes = new byte[640 * 480 * 3];
            voronois.Clear();

            int count = 100;
            for (int i = 0; i < count; i++)
            {
                VoronoiItem voronoi = new VoronoiItem();
                voronoi.cnt = 0;
                voronoi.Rtotal = 0;
                voronoi.Gtotal = 0;
                voronoi.Btotal = 0;
                voronoi.X = rnd.Next(0, 639);
                voronoi.Y = rnd.Next(0, 479);
                voronoi.dx = rnd.Next(0, 2) * 2 - 1;
                voronoi.dy = rnd.Next(0, 2) * 2 - 1;
                voronoi.index = i;
                voronois.Add(voronoi);
            };


            int[,] index2d = new int[640, 480];
            for (int g = 0; g < 1000; g++)
            {
            
                //   0-------0------0
                //   |       |      |
                //   0-------0------0
                //   |       |      |                 
                //   0-------0------0

                CalcPoints.CalcVoronois(voronois, index2d, 0, 0, 640, 480, 0);


                
                FillVoronois(ref voronois, index2d, pixels, ref bytes);
                refreshimage(bytes);

                for (int i = 0; i < count; i++)
                {

                    voronois[i].X = voronois[i].X + voronois[i].dx;
                    voronois[i].Y = voronois[i].Y + voronois[i].dy;
                    if (voronois[i].X > 639)
                    {
                        voronois[i].X = 639;
                        voronois[i].dx = -voronois[i].dx;
                    }
                    if (voronois[i].X < 0)
                    {
                        voronois[i].X = 0;
                        voronois[i].dx = -voronois[i].dx;
                    }
                    if (voronois[i].Y > 479)
                    {
                        voronois[i].Y = 479;
                        voronois[i].dy = -voronois[i].dy;
                    }
                    if (voronois[i].Y < 0)
                    {
                        voronois[i].Y = 0;
                        voronois[i].dy = -voronois[i].dy;
                    }
                    voronois[i].cnt = 0;
                    voronois[i].Rtotal = 0;
                    voronois[i].Gtotal = 0;
                    voronois[i].Btotal = 0;

                }
                PicImage.Refresh();
                stopwatch.Stop();
                label1.Text = stopwatch.ElapsedMilliseconds.ToString();
            }
        }

        private double FillVoronois(ref List<VoronoiItem> voronois,int[,] index2d, byte[,,] inp, ref byte[] outp)
        {                      
            for (int y = 0; y < 480; y++)
                for (int x = 0; x < 640; x++)
                {
                    VoronoiItem voronoi = voronois[index2d[x, y]];
                    voronoi.Rtotal += inp[x, y, 0];
                    voronoi.Gtotal += inp[x, y, 1];
                    voronoi.Btotal += inp[x, y, 2];
                    voronoi.cnt++;
                }
            double err = 0;
            int pos = 0;
            for (int y = 0; y < 480; y++)
                for (int x = 0; x < 640; x++)
                {
                    VoronoiItem voronoi = voronois[index2d[x, y]];
                    outp[pos++] = (byte)(voronoi.Btotal / voronoi.cnt);
                    outp[pos++] = (byte)(voronoi.Gtotal / voronoi.cnt);
                    outp[pos++] = (byte)(voronoi.Rtotal / voronoi.cnt);
                }
            return err;
        }

      
        public void refreshimage(Byte[] binImg)
        {            
            Bitmap bmp = (Bitmap)PicImage.Image;      
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);        
            IntPtr ptr = bmpData.Scan0;   
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;    
            System.Runtime.InteropServices.Marshal.Copy(binImg, 0, ptr, bytes);         
            bmp.UnlockBits(bmpData);
            PicImage.Image = bmp;
        }
    }

    //----------------------------------
    public class VoronoiItem
    {
        public int Rtotal = 0;
        public int Gtotal = 0;
        public int Btotal = 0;
        public int cnt = 0;
        public int X = 0;
        public int Y = 0;
        public int dx = 0;
        public int dy = 0;
        public int index = 0;
    }
}
