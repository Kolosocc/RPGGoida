using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGoida.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int ClassId { get; set; }
        public string SkillEffect { get; set; }
        public int SkillLevel { get; set; }
        public int PersonId { get; set; }

        public Class Class { get; set; }
    }
}
