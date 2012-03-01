using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BabySittingCoop.Web.Models;

namespace BabySittingCoop.Web.Controllers
{
    public class NavigationController : Controller
    {
        [ChildActionOnly]
        public ActionResult Menu(string active)
        {
            var navOptions = new[]
                                 {
                                     new NavigationOption { Text = "Rankings", ActionName = "Index", ControllerName = "Home", IsActive = "Rankings" == active },
                                     new NavigationOption { Text = "Recommendations", ActionName = "Recommendations", ControllerName = "Home", IsActive = "Recommendations" == active }
                                     
                                 };

            return View("Navigation", navOptions);
        }

    }
}
