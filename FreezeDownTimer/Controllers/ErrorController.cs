using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreezeDownTimer.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult ShowError()
        {
            Exception ex = (Exception)TempData["FreezeDownError"];
            ViewBag.Message = ex.Message;

            return View("Error");
        }
    }
}