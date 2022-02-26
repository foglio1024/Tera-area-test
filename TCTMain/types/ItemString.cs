using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTMain
{
    class ItemString
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }

        public ItemString(uint id, string n, string t)
        {
            Id = id;
            Name = n;
            ToolTip = t;
        }
    }
}
