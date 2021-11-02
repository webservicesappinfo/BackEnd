using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillService.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public String Guid { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
    }
}
