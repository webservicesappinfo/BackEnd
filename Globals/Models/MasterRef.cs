using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Models
{
    public class MasterRef<T> : EntityBase where T : EntityBase
    {
        public String Name { get; set; }

        public Guid RefGuid { get; set; }

        public T Parent { get; set; }
    }
}
