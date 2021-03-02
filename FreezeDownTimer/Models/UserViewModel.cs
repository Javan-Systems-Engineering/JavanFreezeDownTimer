using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace FreezeDownTimer.Models
{
    public class UserViewModel
    {
        public int UserID { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]

        public string Password { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EMail { get; set; }

        public bool Active { get; set; }

        public List<Role> assignedRoles { get; set; }

        public IEnumerable<SelectListItem> selectRoles { get; set; }



    }
}