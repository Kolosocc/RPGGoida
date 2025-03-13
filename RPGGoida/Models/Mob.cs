using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class Mob
    {
        public int MobId { get; set; }
        public string MobName { get; set; }
        public int MobLevel { get; set; }
        public int MobDamage { get; set; }
        public string MobType { get; set; }
        public int LocationId { get; set; }

        public Location Location { get; set; }
    }
}
