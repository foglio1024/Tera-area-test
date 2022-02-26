using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCTMain
{
    public class World
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int nameId;
        public int NameId
        {
            get { return nameId; }
            set { nameId = value; }
        }

        private string mapId;
        public string MapId
        {
            get { return mapId; }
            set { mapId = value; }
        }

        private List<Continent> continents;
        public List<Continent> Continents
        {
            get { return continents; }
            set { continents = value; }
        }

        private List<Guard> guards;
        public List<Guard> Guards
        {
            get { return guards; }
            set { guards = value; }
        }

        public Size GetWorldSize()
        {
            double xmin = 0;
            double xmax = 0;
            double ymin = 0;
            double ymax = 0;

            foreach (var guard in guards)
            {
                if (guard.MapData.Position.X > xmax)
                {
                    xmax = guard.MapData.Position.X;
                }
                if (guard.MapData.Position.X < xmin)
                {
                    xmin = guard.MapData.Position.X;
                }
                if (guard.MapData.Position.Y > ymax)
                {
                    ymax = guard.MapData.Position.Y;
                }
                if (guard.MapData.Position.Y < ymin)
                {
                    ymin = guard.MapData.Position.Y;
                }
            }

            return new Size(xmax - xmin, ymax - ymin);
        }

        public Point GetCenter()
        {
            var s = GetWorldSize();
            return new Point(s.Width / 2, s.Height / 2);
        }

        public World(int _id, int _nameId, string _mapId)
        {
            id = _id;
            nameId = _nameId;
            mapId = _mapId;

            continents = new List<Continent>();
            guards = new List<Guard>();
        }
    }
}
