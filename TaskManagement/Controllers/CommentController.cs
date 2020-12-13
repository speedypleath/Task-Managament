using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;
using Microsoft.AspNet.Identity;

namespace TaskManagement.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpPost]
        public ActionResult New(Comment comment)
        {
            try
            {
                comment.UserId = User.Identity.GetUserId();
                comment.Date = DateTime.Now;
                string userId = User.Identity.GetUserId();
                System.Diagnostics.Debug.WriteLine("DAaaa");
                db.Comments.Add(comment);
                db.SaveChanges();
                System.Diagnostics.Debug.WriteLine("wohoo");
                TempData["success"] = "Comment was successfully posted!";
                return RedirectToRoute(new
                {
                    controller = "Task",
                    action = "Show",
                    id = comment.TaskId.ToString()
                });
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                string userId = User.Identity.GetUserId();
                Comment comm = db.Comments.Find(id);
                UserProject user = db.UserProjects.Find(comm.Task.ProjectId, userId);
                if (TryUpdateModel(comm))
                {
                    if (user != null)
                    {
                        if ((comm.UserId != userId) && !User.IsInRole("Admin"))
                        {
                            TempData["fail"] = "You can only edit your comment!";
                            return RedirectToAction("Show/" + comm.TaskId.ToString(), "Task");
                        }
                        comm.Content = requestComment.Content;
                        comm.Date = DateTime.Now;
                        db.SaveChanges();
                        TempData["success"] = "Comment modified successfully!";
                        return RedirectToRoute(new
                        {
                            controller = "Task",
                            action = "Show",
                            id = comm.TaskId.ToString()
                        });
                    }
                    TempData["fail"] = "You're not authorized to see this!";
                    return RedirectToAction("Index", "Project");
                }
                return View();
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
            Comment comment = db.Comments.Find(id);
            UserProject user = db.UserProjects.Find(comment.Task.ProjectId, userId);
            if (user != null)
            {
                if ((comment.UserId != userId) && !User.IsInRole("Admin"))
                {
                    TempData["fail"] = "You can only delete your comment!";
                    return RedirectToAction("Show/" + comment.TaskId.ToString(), "Task");
                }
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["success"] = "Comentariul a fost sters cu succes!";
                return RedirectToRoute(new
                {
                    controller = "Task",
                    action = "Show",
                    id = comment.TaskId.ToString()
                });
            }
            TempData["fail"] = "You're not authorized to see this!";
            return RedirectToAction("Index", "Project");
        }
    }
}