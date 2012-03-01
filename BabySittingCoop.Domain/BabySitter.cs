using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabySittingCoop.Domain
{
    public class BabySitter : DomainBase
    {
        public virtual int? LegacyId { get; set; }
        public virtual string Name { get; set; }
    }

}
