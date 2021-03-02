using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FreezeDownTimer.Models.Interfaces;

namespace FreezeDownTimer.Models
{
    public class Cart : iModficationHistory 
    {
        public int CartID { get; set; }

        public string LineNumber { get; set; }

        public int? CartNumber { get; set; }

        public DateTime InsertDate { get; set; }


        public DateTime? LastUpdate { get; set; }

        public int? LastUpdateBy { get; set; }

        public bool IsDirty { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }

        public virtual ICollection<Location> Location { get; set; }


    }
}