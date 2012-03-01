using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabySittingCoop.Web.Models
{
    public class BabySittingRecommendationsView
    {
        public virtual int BabySitterId { get; set; }
        public virtual string Name { get; set; }
        public virtual int ProvidedCount { get; set; }
    }
}
