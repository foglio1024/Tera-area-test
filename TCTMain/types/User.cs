using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TCTMain
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Guild { get; set; }
        public string Rank { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
        public uint Level { get; set; }
        public UserDot Dot { get; set; }
        public int ServerId { get; set; }

        private uint weaponId;
        private uint armorId;
        private uint glovesId;
        private uint bootsId;
        private uint weaponEnch;

        public string WeaponName
        {
            get
            {
                if (weaponId != 0)
                {
                    var i = ItemsDatabase.Items.Find(x => x.Id == weaponId);
                    return String.Format("+{1} {0} - {2} ({3})", i.Name, weaponEnch, i.ToolTip, i.MwRate);

                }
                else return "Unknown or unequipped";
            }

        }
        public string ArmorName
        {
            get
            {
                if (armorId != 0)
                {
                    var i = ItemsDatabase.Items.Find(x => x.Id == armorId);
                    return String.Format("{0} - {1} ({2})", i.Name, i.ToolTip, i.MwRate);

                }
                else return "Unknown or unequipped";

            }

        }
        public string GlovesName
        {
            get
            {
                if (glovesId != 0)
                {
                    var i = ItemsDatabase.Items.Find(x => x.Id == glovesId);
                    return String.Format("{0} - {1} ({2})", i.Name, i.ToolTip, i.MwRate);

                }
                else return "Unknown or unequipped";

            }

        }
        public string BootsName
        {
            get
            {
                if (bootsId != 0)
                {
                    var i = ItemsDatabase.Items.Find(x => x.Id == bootsId);
                    return String.Format("{0} - {1} ({2})", i.Name, i.ToolTip, i.MwRate);

                }
                else return "Unknown or unequipped";

            }

        }


        public User(long id, string name, string guildName, string guildRank, string race, string gender, string @class, uint level, uint wep, uint ench, uint ch, uint gl, uint bts, Point pos, int heading, int server)
        {
            Id = id;
            Name = name;
            Guild = guildName;
            Rank = guildRank;
            Class = ItemToolTip.Classes.Find(x => x.Name.Equals(@class, StringComparison.OrdinalIgnoreCase)).DisplayedText;
            Level = level;
            Gender = ItemToolTip.Genders.Find(x => x.Name.Equals(gender, StringComparison.OrdinalIgnoreCase)).DisplayedText;

            if (race == "popori" && gender == "female")
            {
                Race = ItemToolTip.Races.Find(x => x.Name.Equals("elin", StringComparison.OrdinalIgnoreCase)).DisplayedText;
            }
            else
            {
                Race = ItemToolTip.Races.Find(x => x.Name.Equals(race, StringComparison.OrdinalIgnoreCase)).DisplayedText; ;
            }

            weaponId = wep;
            weaponEnch = ench;
            armorId = ch;
            glovesId = gl;
            bootsId = bts;
            ServerId = server;
            //Dot = new UserDot(pos, heading, Colors.CadetBlue, Colors.White);
            //AreaWindow.UsersCanvas.Children.Add(Dot);
            //Dot.IsSpawned = true;


        }

        public User(Point pos, int heading)
        {
            Dot = new UserDot(pos, heading, Colors.White, Colors.Black);
            AreaWindow.UsersCanvas.Children.Add(Dot);
            Dot.IsSpawned = true;
        }

        public User(string name)
        {
            Name = name;
        }
    }
}
