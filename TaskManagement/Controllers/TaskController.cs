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
    public class TaskController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Show(int id)
        {
            var task = db.Tasks.Find(id);
            if (TempData.ContainsKey("success"))
            {
                ViewBag.success = TempData["success"].ToString();
            }
            if (TempData.ContainsKey("fail"))
            {
                ViewBag.fail = TempData["fail"].ToString();
            }
            return View(task);
        }

        public ActionResult New(int id)
        {
            string userId = User.Identity.GetUserId();
            var user = db.UserProjects.Find(id, userId);
            if (user != null)
            {
                if (!user.IsEditor && !User.IsInRole("Admin"))
                {
                    TempData["fail"] = "You can only add a task if you're an editor";
                    return RedirectToAction("Show/" + id.ToString(), "Project");
                }
                ViewBag.projectId = id;
                return View();
            }
            TempData["fail"] = "You're not authorized to see this!";
            return RedirectToAction("Index", "Project");

        }

        [HttpPost]
        public ActionResult New(Task task)
        {
            try
            {
                task.Description = "";
                db.Tasks.Add(task);
                db.SaveChanges();
                TempData["success"] = "Taskul a fost adaugat cu succes!";
                return RedirectToRoute(new
                {
                    controller = "Project",
                    action = "Show",
                    id = task.ProjectId.ToString()
                });
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var task = db.Tasks.Find(id);
            string userId = User.Identity.GetUserId();
            var user = db.UserProjects.Find(task.ProjectId, userId);
            if (user != null)
            {
                if (!user.IsEditor && !User.IsInRole("Admin"))
                {
                    TempData["fail"] = "You can only edit a task if you're and editor";
                    return RedirectToAction("Show/" + task.ProjectId.ToString(), "Project");
                }
                return View(task);
            }
            TempData["fail"] = "You're not authorized to see this!";
            return RedirectToAction("Index", "Project");

        }

        [HttpPut]
        public ActionResult Edit(int id, Task requestTask)
        {
            try
            {
                Task task = db.Tasks.Find(id);
                if (TryUpdateModel(task))
                {
                    task.Title = requestTask.Title;
                    task.Description = requestTask.Description;
                    task.DueDate = requestTask.DueDate;
                    db.SaveChanges();
                    TempData["success"] = "Taskul a fost modificat cu succes!";
                }
                return RedirectToAction("Show/" + task.TaskId.ToString());
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var task = db.Tasks.Find(id);
            string userId = User.Identity.GetUserId();
            var user = db.UserProjects.Find(task.ProjectId, userId);
            if (user != null)
            {
                if (!user.IsEditor && !User.IsInRole("Admin"))
                {
                    TempData["fail"] = "You can only delete a task if you're and editor";
                    return RedirectToAction("Show/" + task.ProjectId.ToString(), "Project");
                }
                TempData["success"] = "Taskul a fost sters cu succes!";
                db.Tasks.Remove(task);
                db.SaveChanges();
                return RedirectToRoute(new
                {
                    controller = "Project",
                    action = "Show",
                    id = task.ProjectId.ToString()
                });
            }
            TempData["fail"] = "You're not authorized to see this!";
            return RedirectToAction("Index", "Project");
        }
        public ActionResult Link(Notification note, int TaskId)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            note.Content = user.UserName + note.Content;
            note.Date = DateTime.Now;
            note.Type = "assignment";
            db.Notifications.Add(note);
            UserTask ut = new UserTask();
            ut.TaskId = TaskId;
            ut.UserId = note.UserId;
            db.UserTasks.Add(ut);
            db.SaveChanges();
            return RedirectToAction("/show/" + TaskId.ToString());
        }
        public ActionResult UpdateSearch(int id, string content, string[] selected)
        {
            System.Diagnostics.Debug.WriteLine("daaaa");
            var task = db.Tasks.Find(id);
            var utask = from ut in db.UserTasks
                        where ut.TaskId == id
                        select ut.User;
            ViewBag.members = utask;
            var aux2 = from u in db.Users
                       where !utask.Contains(u)
                       select u;
            var aux = from up in db.UserProjects
                      where up.ProjectId == task.ProjectId
                      select up.User;
            var results = aux.Intersect(aux2);
            var users = results.ToList();
            users = users.OrderBy(each => LevenshteinDistance(each.UserName, content)).ToList();
            return PartialView("_SearchPartial", users.Take(3));
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