using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public Nullable<System.DateTime> SignupDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public Nullable<System.DateTime> BlockedDate { get; set; }
        public bool RememberMe { get; internal set; }
    }
}