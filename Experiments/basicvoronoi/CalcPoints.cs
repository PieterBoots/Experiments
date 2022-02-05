using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace voronoi
{
    class CalcPoints
    {
        public static void CalcVoronois(List<VoronoiItem> voronois, int[,] index2d, int xs, int ys, int W, int H, int Stage)
        {
            //   0-------0------0
            //   |       |      |
            //   0-------0------0
            //   |       |      |                 
            //   0-------0------0
            Square(voronois, index2d, xs, ys, W / 2, H / 2, Stage);
            Square(voronois, index2d, xs + W / 2, ys, W - (W / 2), H / 2, Stage);
            Square(voronois, index2d, xs, ys + H / 2, W / 2, H - (H / 2), Stage);
            Square(voronois, index2d, xs + W / 2, ys + H / 2, W - (W / 2), H - (H / 2), Stage);
        }

        private static void Square(List<VoronoiItem> voronois, int[,] index2d, int offx, int offy, int lw, int lh, int Stage)
        {
            if (Stage < 5)
            {
                //   0-------0
                //   |       |
                //   0-------0

                List<VoronoiItem> sub1 = new List<VoronoiItem>();
                //   0
                //   |      
                //   0
                line(voronois, sub1, lh, offx, offy,  index2d, 0, 1);
                line(voronois, sub1, lh, offx + lw - 1, offy,  index2d, 0, 1);
                //   0-------0          
                line(voronois, sub1, lw, offx, offy,  index2d, 1, 0);
                line(voronois, sub1, lw, offx, offy + lh - 1, index2d, 1, 0);

                //   0-------0
                //   |///////|
                //   0-------0
                for (int i = 0; i < voronois.Count; i++)
                {
                    VoronoiItem v = voronois[i];
                    if ((v.X >= offx) && (v.X <= offx + lw - 1) &&
                        (v.Y >= offy) && (v.Y <= offy + lh - 1))
                        sub1.Add(v);
                }
                CalcVoronois(sub1, index2d, offx + 0, offy + 0, lw, lh, Stage + 1);
            }
            else
            {
                for (int y1 = 0; y1 < lh; y1++)
                    for (int x1 = 0; x1 < lw; x1++)
                        index2d[offx + x1, offy + y1] = Nearest(voronois, offx + x1, offy + y1).index;
            }
        }

        private static void line(List<VoronoiItem> voronois, List<VoronoiItem> sub, int Length, int x, int y, int[,] index2d, int dx, int dy)
        {
            VoronoiItem Prev=null;
            for (int i = 0; i < Length - 1; i++)
            {
                VoronoiItem pick = Nearest(voronois, x, y);
                if (pick != Prev)
                {
                    sub.Add(pick);
                    Prev = pick;
                }
                y += dy;
                x += dx;
            }
        }

        private static VoronoiItem Nearest(List<VoronoiItem> sub, int x, int y)
        {
            double min = 99999999;
            VoronoiItem pick = null;
            for (int t = 0; t < sub.Count; t++)
            {
                VoronoiItem pnt = sub[t];
                double a = (x - pnt.X);
                double b = (y - pnt.Y);
                double dist = a * a + b * b;
                if (dist < min)
                {
                    pick = pnt;
                    min = dist;
                }
            }
            return pick;
        }


    }


}
