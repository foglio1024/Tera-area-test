using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCTMain
{
    public class Location
    {

        private bool isFlying;
        public bool IsFlying
        {
            get { return isFlying; }
            set { isFlying = value; }
        }

        private long id;
        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;

                }
            }
        }

        private Point position;
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        private float height;
        public float Height
        {
            get { return height; }
            set
            {
                if (height == value)
                {
                    height = value;

                }
            }
        }

        private int heading;
        public int Heading
        {
            get { return heading; }
            set
            {

                if (heading == value)
                {
                    heading = value;

                }
            }
        }

        public Location(long id, float tb, float lr, float z, int h, bool f)
        {
            this.id = id;
            position.X = lr;
            position.Y = tb;
            height = z;
            heading = h;
            isFlying = f;
        }
    }
}
