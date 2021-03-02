using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class FreezeDownViewModel
    {
        [Required]
        public string UserName { get; set; }

        public IEnumerable<SelectListItem> UserNames { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Line Number")]
        [Required]
        public string LineNumberStart { get; set; }

        public IEnumerable<SelectListItem> CartLines { get; set; }

        [Display(Name = "Cart Number")]

        [Required]
        public int? CartNumberStart { get; set; }

        public IEnumerable<SelectListItem> CartNumbers { get; set; }


        public string LocationCode { get; set; }

        public string LineNumberEnd { get; set; }

        public int? CartNumberEnd { get; set; }

    }
}