using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class ProfileController : Controller
    {
        // GET: Profile
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            var aux = from u in db.UserProjects
                      where u.UserId == user
                      select u.ProjectId;
            var projects = from p in db.Projects
                       where aux.Contains(p.ProjectId)
                       select p;
            ViewBag.Projects = projects;
            var notifications = from p in db.Notifications
                           where p.UserId == user
                           select p;
            ViewBag.Notifications = notifications;
            return View();
        }
        
    }
}