using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class QuestNpc
    {
        public int QuestNpcId { get; set; }
        public int QuestId { get; set; }
        public int NpcId { get; set; }

        public Quest Quest { get; set; }
        public Npc Npc { get; set; }
    }
}
