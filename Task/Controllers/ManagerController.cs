using System.Linq;
using System.Collections.Generic; 
using System.Data.Entity;
using System.Web.Mvc;
using Task;
using Task.Models; 
using System;
using System.Xml.Linq;
using System.Threading.Tasks;


public class ManagerController : Controller
{
    private readonly TaskManagementDBContext db = new TaskManagementDBContext();

    public string Comments { get; private set; }

    public ActionResult Index()
    {

        return View();
    }
    public ActionResult Home()
    {

        return View();
    }

    public ActionResult ReassignTasks()
    {
        var tasks = db.AssignTasks.ToList(); 
        ViewBag.Users = db.Users.ToList(); 
        return View(tasks); 
    }
    public ActionResult TaskTracking()
    {
        var tasks = db.AssignTasks.ToList(); 
        ViewBag.Users = db.Users.ToList(); 
        return View(tasks); 
    }
    public ActionResult Feedback()
    {
        var tasks = db.AssignTasks.ToList(); 
        ViewBag.Users = db.Users.ToList(); 
        return View(tasks);
    }
    public ActionResult Create()
    {
        if (Session["UserID"] == null || Session["Username"] == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        ViewBag.Users = db.Users.ToList(); 
        return View();
    }

    [HttpPost]
    public ActionResult Create(AssignTasks task, int assignedUserId)
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
    public ActionResult ViewAllTasks()
    {
        if (Session["UserID"] == null || Session["Username"] == null)
        {
            return RedirectToAction("Login", "Account"); 
        }
        var users = db.Users.ToList(); 

        ViewBag.Users = users; 
        string username = (string)Session["Username"];
        var tasks = db.AssignTasks.Where(t => t.CreatedBy == username).ToList(); 
        return View(tasks);
    }


    public ActionResult Reassign(int id)
    {
        var task = db.AssignTasks.Find(id);
        if (task == null)
        {
            return HttpNotFound();
        }

        ViewBag.Users = db.Users.ToList();
        return View(task);
    }

    [HttpPost]
    public ActionResult Reassign(int id, int newAssignedUserId)
    {
        var task = db.AssignTasks.Find(id);
        if (task != null)
        {
            task.AssignedUserID = newAssignedUserId;
            db.SaveChanges();
            var notification = new Notifications
            {
                UserID = newAssignedUserId,  
                Message = $"You have been assigned a new task: {task.Title}",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            db.Notification.Add(notification);
            db.SaveChanges();
            return RedirectToAction("ReassignTasks");
        }
        return View(task);
    }

    public ActionResult OverallMonitoring()
    {
        using (var db = new TaskManagementDBContext())
        {
            
            var tasks = db.AssignTasks.ToList(); 

            
            var activities = db.TaskActivities.ToList();

            
            var viewModel = new TaskMonitoringViewModel
            {
                Tasks = tasks, 
                Activities = activities 
            };

            return View(viewModel);
        }
    }

    public ActionResult ViewTaskDetails(int id)
    {
        using (var db = new TaskManagementDBContext())
        {
            
            var task = db.AssignTasks.FirstOrDefault(t => t.AssignTaskID == id);
            if (task == null)
            {
                return HttpNotFound();
            }

            
            var assignedUsername = db.Users
                .Where(u => u.UserID == task.AssignedUserID)
                .Select(u => u.Username)
                .FirstOrDefault();

            
            var taskActivities = db.TaskActivities
                .Where(a => a.TaskId == id)
                .Select(a => new TaskMonitoringViewModel.ActivityWithUser
                {
                    ActivityId = a.ActivityId,
                    TaskId = a.TaskId,
                    ActivityDescription = a.ActivityDescription,
                    TaskStatus = a.TaskStatus,
                    ProgressPercentage = a.ProgressPercentage,
                    Comments = a.Comments,
                    LastUpdated = a.LastUpdated,
                    UserId = a.UserId,
                    Username = db.Users
                        .Where(u => u.UserID == a.UserId)
                        .Select(u => u.Username)
                        .FirstOrDefault()
                })
                .ToList();

            
            var feedbacks = db.TaskActivities.Where(a => a.TaskId == id).ToList();

            var viewModel = new TaskMonitoringViewModel
            {
                Tasks = new List<AssignTasks> { task },
                AssignedUsername = assignedUsername,
                Activity = taskActivities,
                NewFeedback = new TaskMonitoringViewModel.TaskActivityFeedback
                {
                    TaskId = id,
                    LastUpdated = DateTime.Now
                },
                
                Activities = feedbacks
            };

            return View(viewModel);
        }
    }

    [HttpGet]
    public ActionResult AddComment(int id)
    {
        
        var task = db.AssignTasks.FirstOrDefault(t => t.AssignTaskID == id);
        if (task == null)
        {
            return HttpNotFound();
        }

        ViewBag.TaskTitle = task.Title;
        ViewBag.Username = task.AssignedUserID;
        
       
        var taskActivity = new TaskActivities
        {
            TaskId = task.AssignTaskID,
            TaskStatus= "Feedback Provided"
        };

        return View(taskActivity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddComment(TaskActivities taskActivity)
    {
        if (ModelState.IsValid)
        {
          
            var existingActivity = db.TaskActivities.FirstOrDefault(a => a.TaskId == taskActivity.TaskId);

            if (existingActivity == null)
            {
               
                taskActivity.LastUpdated = DateTime.Now;
                
                db.TaskActivities.Add(taskActivity);
            }
            else
            {
                
                existingActivity.Comments = taskActivity.Comments; 
                existingActivity.LastUpdated = DateTime.Now;
               
                db.Entry(existingActivity).State = EntityState.Modified;
            }

            db.SaveChanges();

            
            return RedirectToAction("Feedback");
        }

        return View(taskActivity); 
    }
   
}













