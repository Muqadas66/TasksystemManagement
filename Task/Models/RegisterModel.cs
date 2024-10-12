using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task.Models
{
    public class RegisterModel
    {
        
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int Role { get; set; }

        public Nullable<System.DateTime> SignupDate { get; set; }

        public bool IsDeleted { get; set; }  
        public bool IsBlocked { get; set; }  
        public DateTime? DeletedDate { get; set; }  
        public DateTime? BlockedDate { get; set; }

    }
}