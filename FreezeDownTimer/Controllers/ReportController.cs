using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using FreezeDownTimer.Models;
using Javan.CustomHelpers;

namespace FreezeDownTimer.Controllers
{
    public class ReportController : Controller
    {
        private readonly DisconnectedRepository _repo = new DisconnectedRepository();

        // GET: Report

        public ActionResult AuditReport(DateTime? StartDate, DateTime? EndDate )
        {

            AuditReportFilterModel arm = new AuditReportFilterModel();           
 

            arm.StartDate = StartDate;
            arm.EndDate = EndDate;

            if (StartDate != null && EndDate != null)
            {
                ViewBag.StartDate = StartDate.ToString();
                ViewBag.EndDate = EndDate.ToString();
            }
            else
            {
                ViewBag.StartDate = "";
                ViewBag.EndDate = "";

            }
            return View(arm);
        
        }

        [HttpPost]
        public ActionResult FilterAuditReport(DateTime? StartDate, DateTime? EndDate)
        {

            AuditReportFilterModel arm = new AuditReportFilterModel()
            {
                StartDate = StartDate,
                EndDate = EndDate
            };




            return Json(Url.Action("AuditReport", "Report", new {StartDate = StartDate, EndDate = EndDate  }));

        }

        public ActionResult GetAuditReport(string StartDate, string EndDate)
        {
            List<AuditReportModel> reportItems = _repo.GetAuditReport(StartDate, EndDate);

            return Json(reportItems, JsonRequestBehavior.AllowGet);
        }
    }
}