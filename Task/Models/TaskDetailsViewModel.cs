using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task.Models
{
    public class TaskDetailsViewModel
    {
        public AssignTasks Task { get; set; }
        public List<TaskActivities> TaskActivities { get; set; }
    }
}