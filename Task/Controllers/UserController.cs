using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
          
using Microsoft.AspNet.Identity.EntityFramework;
using Task.Models;

namespace Task.Controllers
{
    public class UserController : Controller
    {
        private readonly TaskManagementDBContext db = new TaskManagementDBContext(); 
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserTasks()
        {
            
            var username = Session["Username"] as string;

            if (username == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

           
            var loggedInUser = db.Users.FirstOrDefault(u => u.Username == username);
            if (loggedInUser != null)
            {
                var userId = loggedInUser.UserID;
                
                var assignedTasks = db.AssignTasks.Where(t => t.AssignedUserID == userId).ToList();

                return View(assignedTasks);
            }

            
            return View(new List<AssignTasks>());
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeTaskStatus(int id, string newStatus)
        {
            var task = db.AssignTasks.Find(id);
            if (task != null)
            {
                task.Status = newStatus; 
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserTasks"); 
            }
            return HttpNotFound(); 
        }

        public ActionResult GenerateTask()
        {
            ViewBag.Users = db.Users.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult GenerateTask(AssignTasks task, int assignedUserId)
        {
            var username = Session["Username"] as string;

            if (username == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            if (ModelState.IsValid)
            {
                task.AssignedUserID = assignedUserId;
                task.CreatedBy = username; 
                db.AssignTasks.Add(task);
                db.SaveChanges();
                var notification = new Notifications
                {
                    UserID = assignedUserId,  
                    Message = $"You have been assigned a new task: {task.Title}",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                db.Notification.Add(notification);
                db.SaveChanges();


                return RedirectToAction("Index");
            }


            return View(task);
        }


        public ActionResult ViewCreatedTasks()
{
    var username = Session["Username"] as string;

    if (username == null)
    {
        return RedirectToAction("Login", "Account"); 
    }

    
    var createdTasks = db.AssignTasks.Where(t => t.CreatedBy == username).ToList();

    return View(createdTasks); 
}

        public ActionResult EditTask(int id)
        {
            var username = Session["Username"] as string;

            if (username == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            
            var task = db.AssignTasks.FirstOrDefault(t => t.AssignTaskID == id && t.CreatedBy == username);
            if (task == null)
            {
                return HttpNotFound(); 
            }

            return View(task); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask(AssignTasks task)
        {
            var username = Session["Username"] as string;

            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                
                var existingTask = db.AssignTasks.FirstOrDefault(t => t.AssignTaskID == task.AssignTaskID && t.CreatedBy == username);
                if (existingTask == null)
                {
                    return HttpNotFound(); 
                }

                
                existingTask.Status = task.Status;
                existingTask.Priority = task.Priority;
                existingTask.Deadline = task.Deadline;

                
                db.Entry(existingTask).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ViewCreatedTasks"); 
            }

            return View(task); 
        }

        public ActionResult TaskDetails(int id)
        {
            
            var task = db.AssignTasks.Find(id);

            if (task == null)
            {
                return HttpNotFound();             }

          
            var taskActivities = db.TaskActivities.Where(a => a.TaskId == id).ToList();

            
            var viewModel = new TaskDetailsViewModel
            {
                Task = task,
                TaskActivities = taskActivities
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int taskId, string comment)
        {
            var username = Session["Username"] as string;

            if (username == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            // Fetch user details
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return HttpNotFound();
            }

            
            var taskActivity = new TaskActivities
            {
                TaskId = taskId,
                UserId = user.UserID,
                ActivityDescription = "Commented on task",
                Comments = comment,
                LastUpdated = DateTime.Now
            };

            db.TaskActivities.Add(taskActivity);
            db.SaveChanges();
            var task = db.AssignTasks.FirstOrDefault(t => t.AssignTaskID == taskId);
            if (task != null)
            {
                var notification = new Notifications
                {
                    UserID = (int)task.AssignedUserID,  
                    Message = "You have received feedback on your task: " + task.Title,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                db.Notification.Add(notification);
                db.SaveChanges();  
            }
            return RedirectToAction("TaskDetails", new { id = taskId });
        }

        public ActionResult ViewFeedback()
        {
            var username = Session["Username"] as string;

            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            
            var loggedInUser = db.Users.FirstOrDefault(u => u.Username == username);
            if (loggedInUser == null)
            {
                return HttpNotFound(); 
            }

            var userId = loggedInUser.UserID;

            
            var createdTasks = db.AssignTasks.Where(t => t.CreatedBy == username).ToList();

          
            var assignedTasks = db.AssignTasks.Where(t => t.AssignedUserID == userId).ToList();

            
            var combinedTasks = createdTasks.Concat(assignedTasks).ToList();

            return View(combinedTasks);
        }


        public ActionResult ViewNotifications()
        {
            var username = Session["Username"] as string;

            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var loggedInUser = db.Users.FirstOrDefault(u => u.Username == username);
            if (loggedInUser != null)
            {
                var notifications = db.Notification
                    .Where(n => n.UserID == loggedInUser.UserID)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();

                return View(notifications);
            }

            return View(new List<Notifications>());
        }
        public ActionResult MarkAsRead(int id)
        {
            var notification = db.Notification.Find(id);
            if (notification != null)
            {
                notification.IsRead = true;
                db.SaveChanges();
            }

            return RedirectToAction("ViewNotifications");
        }



    }
}