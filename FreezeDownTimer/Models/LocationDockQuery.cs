using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreezeDownTimer.Models
{
    public class LocationDockQuery
    {
        public int DockingID { get; set; }
        public DateTime EndTime { get; set; }

        public string RemainingTime { get; set; }

        public int LocationID { get; set; }

        
        public string LocationCode { get; set; }

        public string AssociatedLocationCode { get; set; }

        public string FullLocationCode { get; set; }

        public string LineNumber { get; set; }
        public int CartNumber { get; set; }



    }
}