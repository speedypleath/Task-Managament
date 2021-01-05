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
    public class ProjectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Project
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                IOrderedQueryable<Project> projects = from p in db.Projects
                                                       orderby p.Name
                                                       select p;
                ViewBag.Projects = projects;
            }
            else
            {
                string userId = User.Identity.GetUserId();
                IQueryable<int> up = from p in db.UserProjects
                         where p.UserId == userId
                         select p.ProjectId;
                IOrderedQueryable<Project> projects = from project in db.Projects
                                                       where up.Contains(project.ProjectId)
                                                       orderby project.Name
                                                       select project;
                ViewBag.Projects = projects;
            }
            if (TempData.ContainsKey("success"))
            {
                ViewBag.success = TempData["success"].ToString();
            }
            if(TempData.ContainsKey("fail"))
            {
                ViewBag.fail = TempData["fail"].ToString();
            }
            return View();
        }
        public ActionResult Show(int id, string how="List")
        {
            ViewBag.how = how;
            ViewBag.id = id;
            var userId = User.Identity.GetUserId();
            var user = from p in db.Users
                       where p.Id == userId
                       select p;
            ViewBag.Users = user;
            var userprojects = from p in db.UserProjects
                               where p.UserId == userId
                               select p.ProjectId;
            // daca nu apartine proiectului
            if (!userprojects.Contains(id) && !User.IsInRole("Admin"))
            {
                TempData["fail"] = "You're not authorized to see this!";
                return RedirectToAction("Index");
            }
            //pt linkul de invite
            if (TempData.ContainsKey("fail"))
            {
                ViewBag.fail = TempData["fail"].ToString();
            }
            var project = db.Projects.Find(id);

            if (TempData.ContainsKey("success"))
            {
                ViewBag.success = TempData["success"].ToString();
            }
            return View(project);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Project project)
        {
            try
            {
                var userProject = new UserProject();
                userProject.IsEditor = true;
                userProject.UserId = User.Identity.GetUserId();
                project.Date = DateTime.Now;
                db.Projects.Add(project);
                db.UserProjects.Add(userProject);
                db.SaveChanges();
                TempData["success"] = "Proiectul " + project.Name + " a fost adaugat cu succes!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            string userId = User.Identity.GetUserId();
            var user = db.UserProjects.Find(id, userId);
            if (user != null)
            {
                if (!user.IsEditor)
                {
                    TempData["Fail"] = "You can only edit the project if you're and editor";
                    return RedirectToAction("Show/" + id.ToString());
                }
            }
            else if (!User.IsInRole("Admin"))
            {
                TempData["fail"] = "You're not authorized to see this!";
                return RedirectToAction("Index");
            }
            Project project = db.Projects.Find(id);
            return View(project);
            
        }

        [HttpPut]
        public ActionResult Edit(int id, Project requestProject)
        {
            try
            {
                Project project = db.Projects.Find(id);
                if (TryUpdateModel(project))
                {
                    project.Name = requestProject.Name;
                    db.SaveChanges();
                    TempData["success"] = "Proiectul " + project.Name + " a fost modificat cu succes!";
                }
                return RedirectToAction("Show/" + project.ProjectId.ToString());
            }
            catch (Exception)
            {
                return View();
            }
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            string userId = User.Identity.GetUserId();
            var user = db.UserProjects.Find(id, userId);
            if (user != null)
            {
                if (!user.IsEditor)
                {
                    TempData["Fail"] = "You can only delete the project if you're and editor";
                    return RedirectToAction("Show/" + id.ToString());
                }
            }
            else if (!User.IsInRole("Admin"))
            {
                TempData["fail"] = "You're not authorized to see this!";
                return RedirectToAction("Index");
            }
            
            Project project = db.Projects.Find(id);
            TempData["success"] = "Proiectul " + project.Name + " a fost sters cu succes!";
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateSearch(int id,string content, string[] selected)
        {
            var aux = from up in db.UserProjects
                      where up.ProjectId == id
                      select up.User;
            ViewBag.members = aux;
            var aux2 = from u in db.Users
                       where !aux.Contains(u)
                       select u;
            List < ApplicationUser > users = aux2.ToList();
            users = users.OrderBy(each => LevenshteinDistance(each.UserName, content)).ToList();
            return PartialView("_SearchPartial", users.Take(3));
        }

        public ActionResult Join(int id)
        {
            try
            {
                UserProject userProject = new UserProject();
                userProject.ProjectId = id;
                userProject.UserId = User.Identity.GetUserId();
                db.UserProjects.Add(userProject);
                db.SaveChanges();
            }
            catch(Exception)
            {
                TempData["fail"] = "You're already enrolled in this project!";

            }
            return RedirectToAction("Show/" + id.ToString());
        }

        public ActionResult Invite(Notification invitation, int ProjectId)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            invitation.Content = user.UserName + invitation.Content;
            invitation.Date = DateTime.Now;
            invitation.Type = "invitation";
            System.Diagnostics.Debug.WriteLine(invitation.Content);
            db.Notifications.Add(invitation);
             db.SaveChanges();
            return RedirectToAction("/show/"+ProjectId.ToString());
        }

        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }
            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}