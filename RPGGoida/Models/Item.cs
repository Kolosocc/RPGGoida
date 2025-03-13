using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int ItemLevel { get; set; }
        public string ItemEffect { get; set; }
        public string ItemRarity { get; set; }
    }
}
