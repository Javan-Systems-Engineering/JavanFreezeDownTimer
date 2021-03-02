using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FreezeDownTimer.Models.Interfaces;

namespace FreezeDownTimer.Models
{
    public class Docking : iModficationHistory
    {
        public int DockingID { get; set; }
        public int CartID { get; set; }
        public Cart Cart { get; set; }

        public int LocationID { get; set; }
        public Location Location { get; set; }

        public DateTime? StartTime{ get; set; }

        public DateTime? EndTime{ get; set; }

        public bool?  IsActive { get; set; }

        public int ElapsedTimeSeconds{ get; set; }

        public DateTime InsertDate { get; set; }

        public int InsertBy { get; set; }


        public DateTime? LastUpdate { get; set; }

        public int? LastUpdateBy { get; set; }

        public bool IsDirty { get; set; }

    }
}