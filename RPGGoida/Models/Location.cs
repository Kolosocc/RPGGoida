using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int LocationSquare { get; set; }
        public string LocationBiome { get; set; }
        public int LocationLevel { get; set; }

        public List<Person> Persons { get; set; }

        public List<Mob> Mobs { get; set; }
        public List<Npc> Npcs { get; set; }
    }
}
