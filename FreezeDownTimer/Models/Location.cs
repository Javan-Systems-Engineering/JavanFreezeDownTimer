using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FreezeDownTimer.Models.Interfaces;

namespace FreezeDownTimer.Models
{
    public class Location : iModficationHistory
    {
        
        public int LocationID { get; set; }

        public string LocationCode { get; set; }

        public string AssociatedLocationCode { get; set; }

        public bool IsTimed { get; set; }

        public bool IsOcuppied  { get; set; }

        public DateTime InsertDate { get; set; }


        public DateTime? LastUpdate { get; set; }

        public int? LastUpdateBy { get; set; }

        public bool IsDirty { get; set; }

    }
}