using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillService.Models
{
    public class Skill : EntityBase
    {
        public String Name { get; set; }
        public String Description { get; set; }
    }

    public class SkillContext : ContextBase<Skill> { }
}
