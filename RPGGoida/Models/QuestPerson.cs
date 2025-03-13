using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class QuestPerson
    {
        public int QuestPersonId { get; set; }
        public int QuestId { get; set; }
        public int PersonId { get; set; }

        public Quest Quest { get; set; }
        public Person Person { get; set; }
    }
}
