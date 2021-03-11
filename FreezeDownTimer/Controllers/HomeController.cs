using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using System.Diagnostics;
using FreezeDownTimer.Filters;
using FreezeDownTimer.Models;
using Models;
using System;

namespace FreezeDownTimer.Controllers
{
    public class HomeController : Controller
    {

        private readonly DisconnectedRepository _repo = new DisconnectedRepository();

        //[FreezeDown]
        //public ActionResult Index( string subTimer, string subDiv)
        //{
        //    if (subTimer != null)
        //    {

        //        return RedirectToAction("Timer", "Home", new { timer = subTimer, counterdiv = subDiv });

        //    }

        //    return View();
        //}

        [FreezeDown]

        public ActionResult Timer()
        {



            // Create new stopwatch.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            System.Threading.Thread.Sleep(10000);

            stopwatch.Stop();


            ViewBag.ElapsedTime = string.Format("{0:hh\\:mm\\:ss}", stopwatch.Elapsed);


            return View();

        }


        [FreezeDown]
        public ActionResult About()
        {
            if (this.HasRole("Administrator"))
            {
                //Perform additional tasks and/or extract additional data from 
                //database into view model/viewbag due to administrative privileges...                
            }

            return View();
        }

        [FreezeDown]
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Login()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int UserID = _repo.IsValidUser(user);

                    if (UserID > 0)
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, false);
                        return RedirectToAction("Index", "Timer");
                    }
                }
                ModelState.AddModelError("", "invalid UserName or Password");
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}