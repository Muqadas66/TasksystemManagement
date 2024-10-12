using System;
using System.Collections.Generic;
using BCrypt.Net;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Task.Models;
using Microsoft.Ajax.Utilities;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Task.Controllers
{
    public class AdminController : Controller
    {
        private readonly TaskManagementDBContext db = new TaskManagementDBContext();


        //[Authorize(UserRole = "Admin")]
        public ActionResult UserList()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/CreateUser
        public ActionResult CreateUser()
        {
            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
               
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                user.SignupDate = DateTime.Now;
                user.IsBlocked = false;
                user.IsDeleted = false;

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("UserList"); 
            }

            return View(user);
        }

        // GET: Admin/EditUser/5
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Find(user.UserID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }
                
                existingUser.Username = user.Username;
                existingUser.Role = user.Role;
                existingUser.IsBlocked = user.IsBlocked;
                existingUser.IsDeleted = user.IsDeleted;

                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View(user);
        }

        // GET: Admin/DeleteUser/5
        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            if (user != null)
            {
                user.IsDeleted = true;
                db.SaveChanges();
            }
            return RedirectToAction("UserList");
        }


        // GET: Admin/ViewAllTasks
        public ActionResult ViewAllTasks(string searchTerm, string status, int? assignedTo, string priority)
        {
           
            if (Session["UserID"] == null || Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

         
            var users = db.Users.ToList();
            ViewBag.Users = users;

           
            string username = (string)Session["Username"];

         
            var tasks = db.AssignTasks.Where(t => t.CreatedBy == username).ToList();

          
            if (!string.IsNullOrEmpty(searchTerm))
            {
                tasks = tasks.Where(t =>
                    t.Title.Contains(searchTerm) ||
                    t.Description.Contains(searchTerm) ||
                    t.Priority.Contains(searchTerm) ||
                    (ViewBag.Users as List<User>).Any(u => u.UserID == t.AssignedUserID && u.Username.Contains(searchTerm))
                ).ToList();
            }

            
            if (!string.IsNullOrEmpty(status))
            {
                tasks = tasks.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(priority))
            {
                tasks = tasks.Where(t => t.Priority == priority).ToList();
            }

            return View(tasks);
        }









        public ActionResult CreateTask()
        {
            if (Session["UserID"] == null || Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Users = db.Users.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateTask(AssignTasks task, int assignedUserId)
        {
            if (Session["UserID"] == null || Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                task.CreatedBy = (string)Session["Username"];


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

                return RedirectToAction("ViewAllTasks");
            }

            return View(task);
        }

        public ActionResult TaskReport()
        {
        
            var totalTasks = db.TaskViews.Count();

          
            var completedTasks = db.TaskViews.Count(t => t.Status == "Completed");

         
            var inProgressTasks = db.TaskViews.Count(t => t.Status == "In Progress");

         
            var pendingTasks = db.TaskViews.Count(t => t.Status == "Pending");

         
            double completionRate = totalTasks > 0 ? ((double)completedTasks / totalTasks) * 100 : 0;

            
            ViewBag.TotalTasks = totalTasks;
            ViewBag.CompletedTasks = completedTasks;
            ViewBag.InProgressTasks = inProgressTasks;
            ViewBag.PendingTasks = pendingTasks;
            ViewBag.CompletionRate = completionRate;

            return View();
        }

        public ActionResult GenerateReport()
        {
            var reportData = GetReportData();
            var reportFile = ReportGenerator.GeneratePdf(reportData);
            return File(reportFile, "application/pdf", "Report.pdf");
        }

        private IEnumerable<ReportItem> GetReportData()
        {
            
            return db.TaskViews.Select(t => new ReportItem
            {
                TaskName = t.Title,
                DueDate = t.DueDate
            }).ToList();
        }

        private string HashPassword(string password)
        {
            
            return password; 
        }
    }
}
