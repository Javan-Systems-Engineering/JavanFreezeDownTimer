using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace FreezeDownTimer.Models
{
    public class PermissionViewModel
    {
        public int PermissionID { get; set; }

        [Required]
        public string PermissionName { get; set; }

        [DataType(DataType.MultilineText)]
        public string PermissionDescription { get; set; }

        public List<Role> assignedRoles { get; set; }


        public IEnumerable<SelectListItem> selectRoles { get; set; }

    }
}