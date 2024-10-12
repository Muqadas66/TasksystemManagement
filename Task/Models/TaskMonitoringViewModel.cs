using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task.Models
{
    public class TaskMonitoringViewModel 
    {
        public List<AssignTasks> Tasks { get; set; } 
        public List<TaskActivities> Activities { get; set; }
        public List<ActivityWithUser> Activity { get; set; }
        public TaskActivityFeedback NewFeedback { get; set; }
        public string AssignedUsername { get; set; }

        public int AssignedUserId { get; set; } 

        public class ActivityWithUser
        {
            public int ActivityId { get; set; }
            public Nullable<int> TaskId { get; set; }
            public string ActivityDescription { get; set; }
            public string TaskStatus { get; set; }
            public Nullable<int> ProgressPercentage { get; set; }
            public string Comments { get; set; }
            public Nullable<System.DateTime> LastUpdated { get; set; }
            public Nullable<int> UserId { get; set; }
            public string Username { get; set; } 
        }

        public class TaskActivityFeedback
        {
            public int TaskId { get; set; }
            public string ActivityDescription { get; set; }
            public string Comments { get; set; }
            public DateTime LastUpdated { get; set; }
            public int UserId { get; set; }
        }


    }
}