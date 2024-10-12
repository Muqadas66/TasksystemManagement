using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Task; 
namespace Task.Models
{
    public class TaskViewModel
    {
        public IEnumerable<AssignTasks> Tasks { get; set; }
        public List<User> Users { get; set; }
    }
}