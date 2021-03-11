using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Web.Mvc;
using FreezeDownTimer.Models;
using Models;
using Javan.CustomHelpers;
using System.Text.RegularExpressions;
using System.Globalization;
using FreezeDownTimer.Filters;

namespace FreezeDownTimer.Controllers
{

    public class TimerController : Controller
    {

        private readonly DisconnectedRepository _repo = new DisconnectedRepository();

        // GET: Timer
        [FreezeDown]
        public ActionResult Index()
        { 
            try   
            { 

            List<LocationDockQuery> ldq = _repo.GetLocationDockings();

            List<TimerModel> list = new List<TimerModel>();

            foreach (var d in ldq)
            {
                string cn = "";
                if (d.CartNumber < 10)
                {
                    cn = "0" + d.CartNumber.ToString();   
                }
                else
                {
                    cn = d.CartNumber.ToString();
                }

                list.Add(new TimerModel
                {
                    TimeDisplayName = "Display_" + d.LocationCode,
                    //ReleaseDateTime = Convert.ToString(new DateTime(2020, 6, 8, 17, 42, 00))
                    //ReleaseDateTime = Convert.ToString(DateTime.Now.AddSeconds(45))
                    ReleaseDateTime = d.EndTime.ToString(),
                    CartLineNumber = d.LineNumber + " Cart #" + cn,
                    CartDisplayName = "Display_Cart_" + d.LocationCode

                }) ;

            }

            FreezeDownViewModel fdvm = new FreezeDownViewModel();
            fdvm.CartLines = new SelectList(_repo.GetCartLines(), "LineNumber", "LineNumber");
            fdvm.CartNumbers = new SelectList(_repo.GetCartNumbers(), "CartNumber", "CartNumber");
            fdvm.UserNames = new SelectList(_repo.GetUsersForDD(), "UserName", "UserName");

            ViewBag.TimerList = list;
            ViewBag.Controller = this;


                //Response.AddHeader("Refresh", "600");

                //return View("Index",fdvm);
                return View("TestGrid",fdvm);

            }

            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }
        }


        [HttpPost]
        [FreezeDown]
        public ActionResult StartFreeze(FreezeDownViewModel fdvm)
        {
            try
            {

                User user = new User()
                {
                    UserName = fdvm.UserName,
                    Password = fdvm.Password
                };

                int UserID = _repo.IsValidUser(user);
                if (UserID == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Invalid UserName or Password", MediaTypeNames.Text.Plain);
                }


                bool IsCartValid = _repo.IsCartValid(fdvm.LineNumberStart, fdvm.CartNumberStart.GetValueOrDefault());
                if (IsCartValid == false)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Please Select a Valid Cart", MediaTypeNames.Text.Plain);
                }

                bool IsCartInUse = _repo.IsCartInUse(fdvm.LineNumberStart, fdvm.CartNumberStart.GetValueOrDefault());
                if (IsCartInUse == true)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Cart Is In Use - Please Select Another", MediaTypeNames.Text.Plain);
                }


                string LocationCode = fdvm.LocationCode;
                int LocationID = _repo.GetLocationID(LocationCode);


                string LineNumber = fdvm.LineNumberStart;
                int CartNumber = fdvm.CartNumberStart.GetValueOrDefault();
                int CartID = _repo.GetCartID(LineNumber, CartNumber);

                var docking = new Docking();
                docking.LocationID = LocationID;
                docking.CartID = CartID;
                docking.StartTime = DateTime.Now;
                //docking.EndTime = docking.StartTime.GetValueOrDefault().AddHours(5);
                //docking.EndTime = docking.StartTime.GetValueOrDefault().AddMinutes(1);
                docking.EndTime = docking.StartTime.GetValueOrDefault().AddSeconds(90);
                docking.IsActive = true;
                docking.InsertDate = DateTime.Now.ConvertToEST();
                docking.InsertBy = UserID;
                //docking.LastUpdate = DateTime.Now.ConvertToEST();
                //docking.LastUpdateBy = UserID;

                bool rslt = false;


                if (ModelState.IsValid)
                {
                    rslt = _repo.CreateDocking(docking);
                }

                if (rslt == true)
                {
                    return Json(Url.Action("Index", "Timer"));

                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Docking Record Creation In Database Failed", MediaTypeNames.Text.Plain);

                }
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }
        }



        [HttpPost]
        [FreezeDown]
        public ActionResult StartFreeze1(string UserName, string Password, string LocationCode, string CartLine, int CartNumber)
        {
            try
            {

                User user = new User()
                {
                    UserName = UserName,
                    Password = Password
                };


                int UserID = _repo.IsValidUser(user);
                if (UserID == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Invalid UserName or Password", MediaTypeNames.Text.Plain);
                }

                //bool IsViewOnly = false;
                //FreezeDownUser fdu = new FreezeDownUser(UserName);
                //IsViewOnly = fdu.HasRole("OperatorViewOnly");
                //if (IsViewOnly == true)
                //{
                //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    return Json("View Only User", MediaTypeNames.Text.Plain);
                //}


                bool IsCartValid = _repo.IsCartValid(CartLine, CartNumber);
                if (IsCartValid == false)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Please Select a Valid Cart", MediaTypeNames.Text.Plain);
                }

                bool IsCartInUse = _repo.IsCartInUse(CartLine, CartNumber);
                if (IsCartInUse == true)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Cart Is In Use - Please Select Another", MediaTypeNames.Text.Plain);
                }


                int LocationID = _repo.GetLocationID(LocationCode);


                int CartID = _repo.GetCartID(CartLine, CartNumber);

                var docking = new Docking();
                docking.LocationID = LocationID;
                docking.CartID = CartID;
                docking.StartTime = DateTime.Now.ConvertToEST();
                docking.EndTime = docking.StartTime.GetValueOrDefault().AddHours(5);
                //docking.EndTime = docking.StartTime.GetValueOrDefault().AddMinutes(1);
                //docking.EndTime = docking.StartTime.GetValueOrDefault().AddSeconds(120);
                docking.IsActive = true;
                docking.InsertDate = DateTime.Now.ConvertToEST();
                docking.InsertBy = UserID;
                //docking.LastUpdate = DateTime.Now.ConvertToEST();
                //docking.LastUpdateBy = UserID;

                bool rslt = false;

                rslt = _repo.CreateDocking(docking);

                if (rslt == true)
                {
                    return Json(Url.Action("Index", "Timer"));

                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Docking Record Creation In Database Failed", MediaTypeNames.Text.Plain);

                }
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }
        }


        [HttpPost]
        [FreezeDown]
        public ActionResult EndFreeze(FreezeDownViewModel fdvm)
        {
            try
            {
                User user = new User()
                {
                    UserName = fdvm.UserName,
                    Password = fdvm.Password
                };

                int UserID = _repo.IsValidUser(user);
                if (UserID == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Invalid UserName or Password", MediaTypeNames.Text.Plain);
                }

                string CartLine = fdvm.LineNumberEnd;
                int CartNumber = fdvm.CartNumberEnd.GetValueOrDefault();
                string LocationCode = fdvm.LocationCode;

                int CartID = _repo.GetCartID(CartLine, CartNumber);
                int LocationID = _repo.GetLocationID(LocationCode);

                Location loc = _repo.GetLocation(LocationID);
                Docking dock = _repo.GetDocking(CartID, LocationID);

                bool rslt = false;

                if (dock.IsActive == true && loc.IsOcuppied == true)
                {
                    loc.IsOcuppied = false;
                    loc.LastUpdate = DateTime.Now.ConvertToEST();

                    dock.IsActive = false;
                    dock.LastUpdate = DateTime.Now.ConvertToEST();
                    dock.LastUpdateBy = UserID;

                    rslt = _repo.UpdateLocationDock(loc, dock);
                }

                return Json(Url.Action("Index", "Timer"));

            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }

        public ActionResult TestGrid()
        {


            return View();
        }

        public ActionResult TestMenu1()
        {


            return View();
        }


    }
}