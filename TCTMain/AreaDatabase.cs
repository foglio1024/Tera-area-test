using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCTMain
{
    public class AreaDatabase
    {
        public static List<Area> Areas { get; set; }

        static List<XDocument> AreaDocs;

        static string filePath = @"C:\Users\Vincenzo1\Desktop\Progetti VS\TeraDataTools-master\TeraDataTools-master\release\xml\Area";
        static void LoadFiles()
        {
            AreaDocs = new List<XDocument>();

            int i = 0;

            while (File.Exists(Path.Combine(filePath, String.Format("Area-{0}.xml", i))))
            {
                XDocument doc = XDocument.Load(Path.Combine(filePath, String.Format("Area-{0}.xml", i)));
                AreaDocs.Add(doc);
                i++;
            }

        }
        static void ParseFile(XDocument doc)
        {
            foreach (XElement a in doc.Descendants().Where(x => x.Name == "Area"))
            {
                var cId = Convert.ToInt32(a.Attribute("continentId").Value);
                var wId = Convert.ToInt32(a.Attribute("worldMapWorldId").Value);
                var gId = Convert.ToInt32(a.Attribute("worldMapGuardId").Value);

                Area area = new Area(cId, wId, gId);

                foreach (XElement s in a.Descendants().Where(x => x.Name == "Section"))
                {
                    var sId = Convert.ToInt32(s.Attribute("worldMapSectionId").Value);

                    area.SectionIds.Add(sId);
                }

                Areas.Add(area);
            }
        }
        public static void PopulateAreas()
        {
            LoadFiles();

            Areas = new List<Area>();

            foreach (var doc in AreaDocs)
            {
                ParseFile(doc);
            }
        }

        public static Area GetArea(int sId)
        {
            foreach (Area a in Areas)
            {
                if (a.SectionIds.Contains(sId))
                {
                    return a;
                }
            }

            return null;
        }
    }


    public class Area
    {
        public int ContinentId { get; set; }
        public int WorldId { get; set; }
        public int GuardId { get; set; }
        public List<int> SectionIds { get; set; }

        public Area(int cId, int wId, int gId)
        {
            ContinentId = cId;
            WorldId = wId;
            GuardId = gId;

            SectionIds = new List<int>();
        }
    }
}
