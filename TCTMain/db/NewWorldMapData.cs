using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TCTMain
{
    public static class NewWorldMapData
    {
        static XDocument WorldMapData;

        public static List<World> Worlds { get; set; }

        static string SafeGetAttributeValue(XElement e, string attributeName)
        {
            if(e.Attribute(attributeName) != null)
            {
                return e.Attribute(attributeName).Value;
            }
            else
            {
                return "0";
            }
        }
        static double CustomDoubleParse(string s)
        {
            if (s.Contains('.'))
            {
                var x = s.Split('.');
                var places = x[1].Length;
                double whole = Convert.ToDouble(x[0]);
                double decimals = Convert.ToDouble(x[1]);
                return whole + decimals * Math.Pow(10, -places);
            }
            else
            {
                return Convert.ToDouble(s);
            }


        }

        public static void PopulateWorldMapData()
        {
            Worlds = new List<World>();

            WorldMapData = new XDocument();            
            WorldMapData = XDocument.Load(Environment.CurrentDirectory + "/content/tera_database/NewWorldMapData.xml");
            foreach (XElement w in WorldMapData.Descendants().Where(x => x.Name == "World"))
            {
                if (w != null)
                {
                    var w_id = Convert.ToInt32(w.Attribute("id").Value);
                    var w_nameId = Convert.ToInt32(SafeGetAttributeValue(w, "nameId"));
                    var w_mapId = SafeGetAttributeValue(w, "mapId");

                    var world = new World(w_id, w_nameId, w_mapId);

                    foreach (XElement c in w.Descendants().Where(x => x.Name == "Continent"))
                    {
                        if (c != null)
                        {
                            var c_id = Convert.ToInt32(c.Attribute("id").Value);
                            var c_x = Convert.ToDouble(c.Attribute("centerPixelX").Value);
                            var c_y = Convert.ToDouble(c.Attribute("centerPixelY").Value);

                            var continent = new Continent(c_id, c_x, c_y);

                            world.Continents.Add(continent);
                        }
                    }
                    foreach (XElement g in w.Descendants().Where(x => x.Name == "Guard"))
                    {
                        if (g != null)
                        {
                            var g_id = Convert.ToInt32(g.Attribute("id").Value);
                            var g_nameId = Convert.ToInt32(SafeGetAttributeValue(g,"nameId"));



                            string g_mapId = SafeGetAttributeValue(g, "mapId");
                            double  g_h= CustomDoubleParse(SafeGetAttributeValue(g,"height"));
                            double  g_w= CustomDoubleParse(SafeGetAttributeValue(g,"width"));
                            double  g_t= CustomDoubleParse(SafeGetAttributeValue(g,"top"));  //not swapped
                            double g_l = CustomDoubleParse(SafeGetAttributeValue(g, "left"));//

                            MapData g_mapData = new MapData(g_h, g_w, g_t, g_l);

                            var guard = new Guard(g_id, g_nameId, g_mapId, g_mapData);

                            foreach (XElement s in g.Descendants().Where(x => x.Name == "Section"))
                            {
                                if (s != null)
                                {
                                    var s_id = Convert.ToInt32(s.Attribute("id").Value);
                                    var s_nameId = Convert.ToInt32(SafeGetAttributeValue(s, "nameId"));
                                    var s_mapId = SafeGetAttributeValue(s,"mapId");

                                    var s_h = CustomDoubleParse(SafeGetAttributeValue(s,"height"));
                                    var s_w = CustomDoubleParse(SafeGetAttributeValue(s,"width"));
                                    var s_l = CustomDoubleParse(SafeGetAttributeValue(s,"left")); //not swapped
                                    var s_t = CustomDoubleParse(SafeGetAttributeValue(s,"top"));  //

                                    var s_mapData = new MapData(s_h, s_w, s_t, s_l);

                                    var section = new Section(s_id, s_nameId, s_mapId, s_mapData);

                                    guard.Sections.Add(section);

                                }
                            }

                            world.Guards.Add(guard);
                        }
                    }

                    Worlds.Add(world);
                }
            }

        }

        public static Section GetSection(int wId, int gId, int sId)
        {
            return Worlds.Find(x => x.Id == wId).Guards.Find(x => x.Id == gId).Sections.Find(x => x.Id == sId);
        }
        public static Section GetSection(int wId, int gId, string mapId)
        {
            return Worlds.Find(x => x.Id == wId).Guards.Find(x => x.Id == gId).Sections.Find(x => x.MapId == mapId);
        }
        public static Section GetSection(string mapId)
        {
            foreach (World w in Worlds)
            {
                foreach (Guard g in w.Guards)
                {
                    foreach (Section s in g.Sections)
                    {
                        if(s.MapId == mapId)
                        {
                            return s;
                        }
                    }
                }
            }
            return new Section();
        }

        public static Guard GetGuard(string mapId)
        {
            foreach (var w  in Worlds)
            {
                foreach (var g in w.Guards)
                {
                    if(g.MapId == mapId)
                    {
                        return g;
                    }
                }
            }
            return new Guard();
        }
        public static Guard GetGuard(int wId, int gId)
        {
            return Worlds.Find(x => x.Id == wId).Guards.Find(x => x.Id == gId);
        }

    }
}
