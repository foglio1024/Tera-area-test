using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTMain
{
    public class Item
    {
        public uint Id { get; set; }
        public string Icon { get; set; }
        public string Category { get; set; }
        public int Level { get; set; }
        public int Tier { get; set; } //rank
        public int Rarity { get; set; } //rareGrade
        public float MwRate { get; set; } //masterpieceRate
        public bool Awakenable { get; set; }
        public bool EnchantEnable { get; set; }
        public bool Tradable { get; set; }

        public string Name { get; set; }
        public string ToolTip { get; set; }

        public Item(uint id, string icon, string cat, int lv, int tier, int rarity, float mw, bool awaken, bool enchant, bool trade, string name, string tt)
        {
            Id = id;
            Icon = icon;
            Category = cat;
            Level = lv;
            Tier = tier;
            rarity = Rarity;
            MwRate = mw;
            Awakenable = awaken;
            EnchantEnable = enchant;
            trade = Tradable;

            Name = name;
            ToolTip = tt;
        }

    }
}
