using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peril.Models
{
    class MoveModel
    {
        public int howMany { get; set; }
        public Types.Territory From { get; set; }
        public Types.Territory To { get; set; }
    }
}
