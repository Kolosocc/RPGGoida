using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class PersonItem
    {
        public int PersonItemId { get; set; }
        public int PersonId { get; set; }
        public int ItemId { get; set; }

        public Person Person { get; set; }
        public Item Item { get; set; }
    }
}
