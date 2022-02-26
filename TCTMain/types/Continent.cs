using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCTMain
{
    public class Continent
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public List<HuntingZone> HuntingZones { get; set; }

        private double centerPixelX;
        private double centerPixelY;

        public Point Center
        {
            get
            {
                return new Point(centerPixelX, centerPixelY);
            }
            set
            {
                centerPixelX = value.X;
                centerPixelY = value.Y;
            }
        }
        public Continent(int _id, double x, double y)
        {
            id = _id;
            centerPixelX = x;
            centerPixelY = y;

        }
        public Continent(int _id)
        {
            id = _id;
            HuntingZones = new List<HuntingZone>();
        }
    }
}
