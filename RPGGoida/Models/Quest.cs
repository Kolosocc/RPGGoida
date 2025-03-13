using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class Quest
    {
        public int QuestId { get; set; }
        public string QuestName { get; set; }
        public int QuestLevel { get; set; }
        public string QuestReward { get; set; }
    }
}
