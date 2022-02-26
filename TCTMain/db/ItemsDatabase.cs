using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TCTMain
{
    public static class ItemsDatabase
    {
        public static List<Item> Items { get; set; }
        static List<ItemString> ItemStrings { get; set; }
        static List<XDocument> StrSheet_ItemDocs = new List<XDocument>();
        static List<XDocument> ItemDataDocs = new List<XDocument>();
        static string stringFilePath = @"C:\Users\Vincenzo1\Desktop\Progetti VS\TeraDataTools-master\TeraDataTools-master\release\xml\StrSheet_Item";
        static string dataFilePath = @"C:\Users\Vincenzo1\Desktop\Progetti VS\TeraDataTools-master\TeraDataTools-master\release\xml\ItemData";

        static void LoadStringFiles()
        {
            int totalStringFiles = Directory.EnumerateFiles(stringFilePath).Count();

            //StrSheet_ItemDocs = new List<XDocument>();

            int i = 0;

            while (File.Exists(Path.Combine(stringFilePath, String.Format("StrSheet_Item-{0}.xml", i))))
            {
                XDocument doc = XDocument.Load(Path.Combine(stringFilePath, String.Format("StrSheet_Item-{0}.xml", i)));
                StrSheet_ItemDocs.Add(doc);
                UI.LogWrite("Loaded string file {0}/{1}", i+1, totalStringFiles);
                i++;
            }

        }
        static void LoadDataFiles()
        {
            int totalDataFiles = Directory.EnumerateFiles(dataFilePath).Count();

            //ItemDataDocs = new List<XDocument>();

            int i = 0;

            while (File.Exists(Path.Combine(dataFilePath, String.Format("ItemData-{0}.xml", i))))
            {
                XDocument doc = XDocument.Load(Path.Combine(dataFilePath, String.Format("ItemData-{0}.xml", i)));
                ItemDataDocs.Add(doc);
                UI.LogWrite("Loaded data file {0}/{1}", i + 1, totalDataFiles);

                i++;
            }

        }
        static void ParseStrSheetFile(XDocument doc)
        {
            foreach (XElement a in doc.Descendants().Where(x => x.Name == "String"))
            {
                var id = Convert.ToUInt32(a.Attribute("id").Value);
                string toolTip = "";
                if (a.Attributes().Where(x => x.Name=="toolTip").Count() != 0)
                {
                    toolTip = System.Net.WebUtility.HtmlDecode(a.Attribute("toolTip").Value);

                }
                var name = a.Attribute("string").Value;
                if (name == "") {
                    continue;
                }
                ItemString item = new ItemString(id, name, toolTip);
                ItemStrings.Add(item);
            }
        }
        static void ParseDataFile(XDocument doc)
        {
            int i = 1;
            var c = doc.Descendants().Where(x => x.Name == "Item").Count();
            foreach (XElement a in doc.Descendants().Where(x => x.Name == "Item"))
            {
                var id = Convert.ToUInt32(a.Attribute("id").Value);
                var icon = a.Attribute("icon").Value;

                string cat = "none";
                if (a.Attributes().Where(x => x.Name == "category").Count() != 0)
                {
                    cat = a.Attribute("category").Value;
                }

                var lv = 0;
                if(a.Attributes().Where(x => x.Name == "level").Count() != 0)
                {
                    lv = Convert.ToInt32(a.Attribute("level").Value);
                }

                var tier = Convert.ToInt32(a.Attribute("rank").Value);
                var rarity = Convert.ToInt32(a.Attribute("rareGrade").Value);

                bool awaken = false;
                if(a.Attributes().Where(x => x.Name == "awakenable").Count() != 0)
                {
                    awaken = ParseBool(a.Attribute("awakenable").Value);
                }

                var enchant = ParseBool(a.Attribute("enchantEnable").Value);
                var trade = ParseBool(a.Attribute("tradable").Value);

                float mwRate = 0;
                if (a.Attributes().Where(x => x.Name == "masterpieceRate").Count() != 0)
                {
                    mwRate = CustomFloatParse(a.Attribute("masterpieceRate").Value);
                }

                string name = "Unknown";
                string tt = "Unknown";
                if(ItemStrings.Find( x => x.Id == id) != null)
                {
                    name = ItemStrings.Find(x => x.Id == id).Name;
                    tt = ItemStrings.Find(x => x.Id == id).ToolTip;
                }

                var item = new Item(id, icon, cat, lv, tier, rarity, mwRate, awaken, enchant, trade, name, tt);
                Items.Add(item);
                //Console.Write("\r[{1}/{2}] Added item: {0}", item.Name, i,c);
                i++;
            }
        }

        static float CustomFloatParse(string s)
        {
            if (s.Contains('.'))
            {
                var x = s.Split('.');
                var places = x[1].Length;
                float whole = Convert.ToSingle(x[0]);
                float decimals = Convert.ToSingle(x[1]);
                return Convert.ToSingle(whole + decimals * Math.Pow(10, -places));
            }
            else
            {
                return Convert.ToSingle(s);
            }


        }

        public static void PopulateItems()
        {
            LoadStringFiles();
            LoadDataFiles();
            ItemStrings = new List<ItemString>();
            Items = new List<Item>();
            int i = 1;
            foreach (var doc in StrSheet_ItemDocs)
            {
                ParseStrSheetFile(doc);
                UI.LogWrite("Parsing string file {0}/{1}",i, StrSheet_ItemDocs.Count);
                i++;
            }
            i = 1;
            foreach (var doc in ItemDataDocs)
            {
                UI.LogWrite("Parsing file {0}/{1}", i,ItemDataDocs.Count);
                ParseDataFile(doc);
                i++;
            }

            ItemStrings.Clear();
            UI.LogWrite("Items database loaded.");
        }
        static bool ParseBool(string attr)
        {
            if (attr.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else return true;
        }
    }


}
