using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabySittingCoop.Domain
{
    public class BabySittingTransaction : DomainBase
    {
        public virtual int ChildrenWatched { get; set; }

        public virtual TimeSpan Duration { get; set; }

        public virtual BabySitter SittingProvider { get; set; }

        public virtual BabySitter SittingReceiver { get; set; }

        public virtual DateTimeOffset StartedAtUtc { get; set; }
    }
}
