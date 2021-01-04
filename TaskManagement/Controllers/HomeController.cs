using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated) return RedirectToRoute(new
            {
                controller = "Project",
                action = "Index"
            });

            return RedirectToRoute(new
            {
                controller = "Account",
                action = "Login"
            });
        }
    }
}