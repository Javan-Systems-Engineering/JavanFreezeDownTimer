using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace FreezeDownTimer.Models
{
    public class RoleViewModel
    {
        public int RoleID { get; set; }

        [Required]
        public string RoleName { get; set; }

        [DataType(DataType.MultilineText)]
        public string RoleDescription { get; set; }
        public bool IsSysAdmin { get; set; }

        public List<Permission> assignedPermissions { get; set; }


        public IEnumerable<SelectListItem> selectPermissions { get; set; }

    }
}