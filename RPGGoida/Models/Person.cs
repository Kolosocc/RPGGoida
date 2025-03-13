using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public int UserId { get; set; }
        public string PersonName { get; set; }
        public int PersonLevel { get; set; } = 1;
        public string PersonNation { get; set; }
        public string PersonGender { get; set; }
        public int ClassId { get; set; }  
        public int LocationId { get; set; }

        public User User { get; set; }
        public Class Class { get; set; }
        public List<PersonItem> PersonItems { get; set; }
    }
}