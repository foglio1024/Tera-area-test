using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCTMain
{
    public class MapData
    {
        public static double Scale { get; set; } 

        private double h;
        private double w;
        private double t;
        private double l;

        public Point Position {
            get
            {
                return new Point(l, t);
            }

            set
            {
                l = value.X;
                t = value.Y;
            }
        }
        public Size Size { get { return new Size(w, h); } }
        public Point BottomLeftCorner { get {
                return new Point
                {
                    X = l,
                    Y = t - h
                };
            }
        }
        public MapData(double _h, double _w, double _t, double _l)
        {
            this.h = _h;
            this.w = _w;
            this.t = _t;
            this.l = _l;
        }

        public MapData()
        {
            h = w = t = l = 0;
            Scale = .1;
        }
    }
}
