using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreezeDownTimer.Models
{
    public class AuditReportModel
    {
        public string UserName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string LocationCode { get; set; }

        public string Cart { get; set; }

        public DateTime? CartRemoved { get; set; }


    }
}