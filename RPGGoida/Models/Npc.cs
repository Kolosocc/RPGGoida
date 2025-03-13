using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class Npc
    {
        public int NpcId { get; set; }
        public string NpcName { get; set; }
        public int NpcLevel { get; set; }
        public int LocationId { get; set; }

        public Location Location { get; set; }
    }
}
