using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabySittingCoop.Domain.Views
{
    public class BabySittingPointsView : DomainBase
    {
        public virtual string Name { get; set; }
        public virtual int ProvidedPoints { get; set; }
        public virtual int ReceiverPoints { get; set; }
        public virtual int TotalPoints { get; set; }
    }
}
