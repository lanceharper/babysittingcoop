using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabySittingCoop.Domain
{
    public abstract class DomainBase : IId
    {
        public virtual int Id { get; set; }
    }
}
