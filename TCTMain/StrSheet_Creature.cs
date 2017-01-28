using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCTMain
{
    public static class StrSheet_Creature
    {
        static XDocument StrSheetCreature;

        public static List<HuntingZone> HuntingZones { get; set; }

        public static void PopulateHuntingZones()
        {
            HuntingZones = new List<HuntingZone>();

            StrSheetCreature = new XDocument();
            StrSheetCreature = XDocument.Load(@"C:\Users\Vincenzo1\Desktop\Progetti VS\TeraDataTools-master\TeraDataTools-master\release\xml\StrSheet_Creature.xml");
            foreach (XElement hz in StrSheetCreature.Descendants().Where(x => x.Name == "HuntingZone"))
            {
                var hzId = Convert.ToInt32(hz.Attribute("id").Value);

                HuntingZone huntingZone = new HuntingZone(hzId);

                foreach (XElement n in hz.Descendants().Where(x => x.Name == "String"))
                {
                    var n_name = n.Attribute("name").Value;
                    var n_id = Convert.ToInt32(n.Attribute("templateId").Value);

                    Npc npc = new Npc(n_name, n_id);

                    huntingZone.Npcs.Add(npc);
                }

                HuntingZones.Add(huntingZone);
            }

        }

        public static string GetNpcName(int zoneId, uint templateId)
        {
            string npcName = "Unknown";

            if (HuntingZones.Find(x => x.Id == zoneId) != null)
            {
                var zone = HuntingZones.Find(x => x.Id == zoneId);
                
                if(zone.Npcs.Find(x => x.TemplateId == templateId) != null)
                {
                    var npc = zone.Npcs.Find(x => x.TemplateId == templateId);
                    npcName = npc.Name;
                }
            }
            return npcName;
        }

        public static Dictionary<int, Npc> GetNpcsByTemplateId(uint templateId)
        {
            Dictionary<int, Npc> result = new Dictionary<int, Npc>();
            foreach (var HZ in HuntingZones)
            {
                foreach (var npc in HZ.Npcs)
                {
                    if(npc.TemplateId == templateId)
                    {
                        result.Add(HZ.Id, npc);
                    }
                }
            }
            return result;
        }
    }

    public class Npc
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }

        public Npc(string n, int id)
        {
            Name = n;
            TemplateId = id;
        }
    }
}
