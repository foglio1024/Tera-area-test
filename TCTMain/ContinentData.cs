using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCTMain
{
    public static class ContinentData
    {

        static XDocument ContinentDataDocument;

        public static List<Continent> Continents;

        public static void PopulateContinentData()
        {
            ContinentDataDocument = new XDocument();
            ContinentDataDocument = XDocument.Load(@"C:\Users\Vincenzo1\Desktop\Progetti VS\TeraDataTools-master\TeraDataTools-master\release\xml\ContinentData.xml");

            Continents = new List<Continent>();

            foreach (XElement c in ContinentDataDocument.Descendants().Where(x => x.Name == "Continent"))
            {
                var c_id = Convert.ToInt32(c.Attribute("id").Value);
                var continent = new Continent(c_id);

                foreach (XElement hz in c.Descendants().Where(x => x.Name == "HuntingZone"))
                {
                    var hz_id = Convert.ToInt32(hz.Attribute("id").Value);
                    var huntingZone = new HuntingZone(hz_id);
                    continent.HuntingZones.Add(huntingZone);
                }

                Continents.Add(continent);
            }
        }
    }
}
