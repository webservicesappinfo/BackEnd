using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Models
{
    public class OfferRef<T>: EntityBase where T: EntityBase 
    {
        public Guid RefGuid { get; set; }
        public String Name { get; set; }    
        public T Parent { get; set; }
    }
}
