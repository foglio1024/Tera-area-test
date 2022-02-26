using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTMain
{
    public class HuntingZone
    {

        public int Id { get; set; }

        public List<Npc> Npcs { get; set; }

        public HuntingZone(int _id)
        {
            Id = _id;
            Npcs = new List<Npc>();
        }
    }
}
