using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTMain
{
    public class Guard
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

        private MapData mapData;
        public MapData MapData
        {
            get { return mapData; }
            set { mapData = value; }
        }

        private List<Section> sections;
        public List<Section> Sections
        {
            get { return sections; }
            set { sections = value; }
        }

        public Guard(int _id, int _nameId, string _mapId, MapData _mapData)
        {
            id = _id;
            nameId = _nameId;
            mapId = _mapId;
            mapData = _mapData;

            sections = new List<Section>();
        }

        public Guard()
        {
        }
    }
}
