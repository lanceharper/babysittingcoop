using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabySittingCoop.Web.Models
{
    public class NavigationOption
    {
        public string Text { get; set; }
        public bool IsActive { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}