using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FreezeDownTimer.Models;
using Models;
using Javan.CustomHelpers;
using FreezeDownTimer.Filters;
using System.Data.Entity;

namespace FreezeDownTimer.Controllers
{
    public class Admin2Controller : Controller
    {

        private readonly DisconnectedRepository _repo = new DisconnectedRepository();

        private FreezeDownContext database = new FreezeDownContext();

        #region Users
        // GET: Admin2
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [FreezeDown]

        public ActionResult Users()
        {
            return View();

        }

        public ActionResult GetUserList()
        {
            List<UserViewModel> users = _repo.GetUserList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [FreezeDown]

        public ActionResult UserEdit(int UserID) 
        {

            var user = _repo.GetUserByID(UserID);


            UserViewModel uvm = new UserViewModel()
            {
                UserID = user.UserID,
                UserName = user.UserName.Trim(),
                Active = !(user.Inactive ?? false),
                LastName = user.LastName.Trim(),
                FirstName = user.FirstName.Trim(),
                Password = user.Password.Trim(),
                EMail = String.IsNullOrEmpty(user.EMail) ? "" : user.EMail.Trim(),
                assignedRoles = user.Roles.ToList()
            };
        
            uvm.selectRoles = new SelectList(_repo.GetRoles(), "RoleID", "RoleName");

            bool IsSaved;
            if (TempData["IsSaved"] != null)
            {
                IsSaved = (bool)TempData["IsSaved"];
                ViewBag.IsSaved = IsSaved;
            }
            else
            {
                ViewBag.IsSaved = false;
            }


            return View("UserEdit", uvm);

        }

        [FreezeDown]
        [HttpPost]
        public ActionResult UserEditDB(UserViewModel uvm)
        {
            try
            {
                User currentUser = _repo.GetUserByID(uvm.UserID);

                string loggedinUserName = FreezeDown_ExtendedMethods.GetUID();
                int loggedinUID = new FreezeDownUser(loggedinUserName).UserID;

                bool rslt = false;

                if (currentUser != null)
                {
                    currentUser.UserName = uvm.UserName;
                    currentUser.Password = uvm.Password;
                    currentUser.LastName = uvm.LastName;
                    currentUser.FirstName = uvm.FirstName;
                    currentUser.EMail = uvm.EMail;
                    currentUser.Inactive = !uvm.Active;
                    currentUser.LastUpdate = DateTime.Now.ConvertToEST();
                    currentUser.LastUpdateBy = loggedinUID;

                    
                }

                rslt = _repo.UpdateUser(currentUser);

                TempData["IsSaved"] = true;


                return RedirectToAction("UserEdit","Admin2", new { UserID = currentUser.UserID });
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }
        }

        [FreezeDown]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUserRoleReturnPartialView(int RoleID, int UserID)
        {
            Role role = database.Roles.Find(RoleID);
            User user = database.Users.Find(UserID);


            bool rslt = false;

            if (!role.Users.Contains(user))
            {
                rslt = _repo.AddUserRole(RoleID, UserID);
            }
            
            if (rslt == true)
            {
                ViewBag.IsSaved = true;
            
            }

            user = _repo.GetUserByID(UserID);

            UserViewModel uvm = MapUsertoUVM(user);

            return PartialView("_UserRoleTable",uvm );

        }

        [FreezeDown]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserRoleReturnPartialView(int RoleID, int UserID)
        {
            Role role = database.Roles.Find(RoleID);
            User user = database.Users.Find(UserID);


            if (role.Users.Contains(user))
            {
                role.Users.Remove(user);
                database.SaveChanges();
            }

            ViewBag.IsSaved = true;


            user = _repo.GetUserByID(UserID);

            UserViewModel uvm = MapUsertoUVM(user);

            return PartialView("_UserRoleTable", uvm);
        }

        [FreezeDown]
        public ActionResult UserCreate()
        {

            UserViewModel uvm = new UserViewModel();


            return View(uvm);
        
        }

        [HttpPost]
        public ActionResult UserCreateDB(UserViewModel uvm)
        {

            try
            {

                User user = MapUVMtoUser(uvm);

                if (user.UserName == "" || user.UserName == null)
                {
                    ModelState.AddModelError(string.Empty, "UserName cannot be blank");
                }


                if (ModelState.IsValid)
                {

                    int UserID = GetLoggedInUID();

                    List<string> results = database.Database.SqlQuery<String>(string.Format("SELECT UserName FROM [User] WHERE UserName = '{0}'", user.UserName)).ToList();
                    bool _userExistsInTable = (results.Count > 0);

                    User _user = null;
                    if (_userExistsInTable)
                    {
                        _user = database.Users.Where(p => p.UserName == user.UserName).FirstOrDefault();
                        if (_user != null)
                        {
                            if (_user.Inactive == false)
                            {
                                ModelState.AddModelError(string.Empty, "User already exists!");
                            }
                            else
                            {
                                database.Entry(_user).Entity.Inactive = false;
                                database.Entry(_user).Entity.InsertDate = DateTime.Now.ConvertToEST();
                                database.Entry(_user).Entity.LastUpdate = DateTime.Now.ConvertToEST();
                                database.Entry(_user).Entity.LastUpdateBy = UserID;
                                database.Entry(_user).State = EntityState.Modified;
                                database.SaveChanges();
                                return RedirectToAction("Users");
                            }
                        }
                    }
                    else
                    {
                        _user = new User();
                        _user.UserName = user.UserName;
                        _user.LastName = user.LastName;
                        _user.FirstName = user.FirstName;
                        _user.EMail = user.EMail;
                        _user.Password = user.Password;

                        if (ModelState.IsValid)
                        {
                            _user.Inactive = false;
                            _user.InsertDate = DateTime.Now.ConvertToEST();
                            _user.LastUpdate = DateTime.Now.ConvertToEST();
                            _user.LastUpdateBy = UserID;

                            database.Users.Add(_user);
                            database.SaveChanges();
                            return RedirectToAction("Users");
                        }
                    }

                }


                return View("UserCreate",uvm);
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }

        public UserViewModel MapUsertoUVM(User user)
        {

            UserViewModel uvm = new UserViewModel()
            {
                UserID = user.UserID,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password,
                EMail = user.EMail,
                Active = !(bool)user.Inactive,
                assignedRoles = user.Roles.ToList()

            };

            return uvm;

        }

        public User MapUVMtoUser(UserViewModel uvm)
        {

            User user = new User()
            {
                UserID = uvm.UserID,
                UserName = uvm.UserName,
                FirstName = uvm.FirstName,
                LastName = uvm.LastName,
                Password = uvm.Password,
                EMail = uvm.EMail,
                Inactive = !(bool)uvm.Active,

            };

            return user;

        }

        #endregion

        #region Roles

        [FreezeDown]
        public ActionResult Roles()
        {


            return View();


        }

        [FreezeDown]
        public ActionResult GetRoleList()
        {
            List<RoleViewModel> roles = _repo.GetRoleList();

            return Json(roles, JsonRequestBehavior.AllowGet);
        }


        [FreezeDown]
        public ActionResult RoleEdit(int RoleID)
        {

            var role = _repo.GetRoleByID(RoleID);


            RoleViewModel rvm = new RoleViewModel()
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName.Trim(),
                RoleDescription = role.RoleDescription,
                IsSysAdmin = role.IsSysAdmin,
                assignedPermissions = role.Permissions.OrderBy(c => c.PermissionName).ToList()
            };

            rvm.selectPermissions = new SelectList(_repo.GetPermissions(), "PermissionID", "PermissionName");

            bool IsSaved;
            if (TempData["IsSaved"] != null)
            {
                IsSaved = (bool)TempData["IsSaved"];
                ViewBag.IsSaved = IsSaved;
            }
            else
            {
                ViewBag.IsSaved = false;
            }


            return View("RoleEdit", rvm);

        }

        [HttpPost]
        public ActionResult RoleEditDB(RoleViewModel rvm)
        {
            try
            {
                Role role = _repo.GetRoleByID(rvm.RoleID);

                string loggedinUserName = FreezeDown_ExtendedMethods.GetUID();
                int loggedinUID = new FreezeDownUser(loggedinUserName).UserID;

                bool rslt = false;

                if (role != null)
                {
                    role.RoleName = rvm.RoleName;
                    role.RoleDescription = rvm.RoleDescription;
                    role.IsSysAdmin = rvm.IsSysAdmin;
                    role.LastUpdate = DateTime.Now.ConvertToEST();
                    role.LastUpdateBy = loggedinUID;
                }

                rslt = _repo.UpdateRole(role);

                TempData["IsSaved"] = true;


                return RedirectToAction("RoleEdit", "Admin2", new { RoleID = role.RoleID });
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRolePermissionReturnPartialView(int RoleID, int PermissionID)
        {
            Role role = database.Roles.Find(RoleID);
            Permission permission = database.Permissions.Find(PermissionID);

            bool rslt = false;

            if (!permission.Roles.Contains(role))
            {
                rslt = _repo.AddRolePermission(RoleID, PermissionID);
            }

            if (rslt == true)
            {
                ViewBag.IsSaved = true;

            }

            role = _repo.GetRoleByID(RoleID);

            RoleViewModel rvm = new RoleViewModel()
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                RoleDescription = role.RoleDescription,
                IsSysAdmin = role.IsSysAdmin,
                assignedPermissions = role.Permissions.OrderBy(c => c.PermissionName ).ToList()
            };

            return PartialView("_RolePermissionTable", rvm);

        }


        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRolePermissionReturnPartialView(int RoleID, int PermissionID)
        {
            Role role = database.Roles.Find(RoleID);
            Permission permission = database.Permissions.Find(PermissionID);


            if (role.Permissions.Contains(permission))
            {
                role.Permissions.Remove(permission);
                database.SaveChanges();
            }

            ViewBag.IsSaved = true;


            role = database.Roles.Find(RoleID);

            RoleViewModel rvm = new RoleViewModel()
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                RoleDescription = role.RoleDescription,
                IsSysAdmin = role.IsSysAdmin,
                assignedPermissions = role.Permissions.ToList()

            };

            return PartialView("_RolePermissionTable", rvm);
        }

        [FreezeDown]
        public ActionResult RoleCreate()
        {

            RoleViewModel rvm = new RoleViewModel();


            return View(rvm);

        }

        [HttpPost]
        public ActionResult RoleCreateDB(RoleViewModel rvm)
        {

            try
            {

                Role role = new Role()
                {
                    RoleName = rvm.RoleName,
                    RoleDescription = rvm.RoleDescription,
                    IsSysAdmin = rvm.IsSysAdmin
                };

                if (role.RoleName == "" || role.RoleName == null)
                {
                    ModelState.AddModelError(string.Empty, "Role Name cannot be blank");
                }


                if (ModelState.IsValid)
                {

                    int UserID = GetLoggedInUID();

                    List<string> results = database.Database.SqlQuery<String>(string.Format("SELECT RoleName FROM [Role] WHERE RoleName = '{0}'", role.RoleName)).ToList();
                    bool _roleExistsInTable = (results.Count > 0);

                    Role _role = null;

                    if (_roleExistsInTable)
                    {

                            ModelState.AddModelError(string.Empty, "Role already exists!");                        
                    }
                    else
                    {
                        _role = new Role();
                        _role.RoleName = role.RoleName;
                        _role.RoleDescription = role.RoleDescription;
                        _role.IsSysAdmin = role.IsSysAdmin;

                        if (ModelState.IsValid)
                        {
                            _role.InsertDate = DateTime.Now.ConvertToEST();
                            _role.LastUpdate = DateTime.Now.ConvertToEST();
                            _role.LastUpdateBy = UserID;

                            database.Roles.Add(_role);
                            database.SaveChanges();
                            return RedirectToAction("Roles");
                        }
                    }

                }


                return View("RoleCreate", rvm);
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }


        [HttpPost]
        public ActionResult RoleDeleteDB(int RoleID)
        {

            Role role = database.Roles.Find(RoleID);

            List<Permission> permissions = role.Permissions.ToList();
            foreach (var p in permissions)
            {
                role.Permissions.Remove(p);
                database.SaveChanges();
            }

            List<User> users = role.Users.ToList();
            foreach (var u in users)
            {
                role.Users.Remove(u);
                database.SaveChanges();
            }

            database.Roles.Remove(role);
            database.SaveChanges();

            return Json(Url.Action("Roles", "Admin2"));

        }

        #endregion

        #region Permissions

        [FreezeDown]
        public ActionResult Permissions()
        {

            return View();
        }

        public ActionResult GetPermissionList()
        {
            List<PermissionViewModel> permissions = _repo.GetPermissionList();

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        [FreezeDown]
        public ActionResult PermissionCreate()
        {

            PermissionViewModel pvm = new PermissionViewModel();


            return View(pvm);

        }



        [HttpPost]
        public ActionResult PermissionCreateDB(PermissionViewModel pvm)
        {

            try
            {

                Permission permission = new Permission()
                {
                    PermissionName = pvm.PermissionName,
                    PermissionDescription = pvm.PermissionDescription
                };

                if (permission.PermissionName == "" || permission.PermissionName == null)
                {
                    ModelState.AddModelError(string.Empty, "Permission Name cannot be blank");
                }


                if (ModelState.IsValid)
                {

                    int UserID = GetLoggedInUID();

                    List<string> results = database.Database.SqlQuery<String>(string.Format("SELECT PermissionName FROM [Permission] WHERE PermissionName = '{0}'", permission.PermissionName)).ToList();
                    bool _permissionExistsInTable = (results.Count > 0);

                    Permission _permission = null;

                    if (_permissionExistsInTable)
                    {

                        ModelState.AddModelError(string.Empty, "Permission already exists!");
                    }
                    else
                    {
                        _permission = new Permission();
                        _permission.PermissionName = permission.PermissionName;
                        _permission.PermissionDescription = permission.PermissionDescription;

                        if (ModelState.IsValid)
                        {
                            _permission.InsertDate = DateTime.Now.ConvertToEST();
                            _permission.LastUpdate = DateTime.Now.ConvertToEST();
                            _permission.LastUpdateBy = UserID;

                            database.Permissions.Add(_permission);
                            database.SaveChanges();
                            return RedirectToAction("Permissions");
                        }
                    }

                }


                return View("PermissionCreate", pvm);
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }

        public ActionResult PermissionDeleteDB(int PermissionID)
        {

            Permission permission = database.Permissions.Find(PermissionID);

            List<Role> roles = permission.Roles.ToList();
            foreach (var r in roles)
            {
                permission.Roles.Remove(r);
                database.SaveChanges();
            }


            database.Permissions.Remove(permission);
            database.SaveChanges();

            return Json(Url.Action("Permissions", "Admin2"));

        }

        [FreezeDown]
        public ActionResult PermissionEdit(int PermissionID)
        {

            var permission = _repo.GetPermissionByID(PermissionID);


            PermissionViewModel pvm = new PermissionViewModel()
            {
                PermissionID = permission.PermissionID,
                PermissionName = permission.PermissionName.Trim(),
                PermissionDescription = permission.PermissionDescription,
                assignedRoles = permission.Roles.ToList()
            };

            pvm.selectRoles = new SelectList(_repo.GetRoles(), "RoleID", "RoleName");

            bool IsSaved;
            if (TempData["IsSaved"] != null)
            {
                IsSaved = (bool)TempData["IsSaved"];
                ViewBag.IsSaved = IsSaved;
            }
            else
            {
                ViewBag.IsSaved = false;
            }


            return View("PermissionEdit", pvm);

        }


        [HttpPost]
        public ActionResult PermissionEditDB(PermissionViewModel pvm)
        {
            try
            {
                Permission permission = _repo.GetPermissionByID(pvm.PermissionID);

                string loggedinUserName = FreezeDown_ExtendedMethods.GetUID();
                int loggedinUID = new FreezeDownUser(loggedinUserName).UserID;

                bool rslt = false;

                if (permission != null)
                {
                    permission.PermissionName = pvm.PermissionName;
                    permission.PermissionDescription = pvm.PermissionDescription;
                    permission.LastUpdate = DateTime.Now.ConvertToEST();
                    permission.LastUpdateBy = loggedinUID;
                }

                rslt = _repo.UpdatePermission(permission);

                TempData["IsSaved"] = true;


                return RedirectToAction("PermissionEdit", "Admin2", new { PermissionID = permission.PermissionID });
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }
        }



        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddPermissionRoleReturnPartialView( int PermissionID, int RoleID)
        {
            Role role = database.Roles.Find(RoleID);
            Permission permission = database.Permissions.Find(PermissionID);

            bool rslt = false;

            if (!role.Permissions.Contains(permission))
            {
                rslt = _repo.AddPermissionRole(PermissionID, RoleID );
            }

            if (rslt == true)
            {
                ViewBag.IsSaved = true;

            }

            permission = _repo.GetPermissionByID(PermissionID);

            PermissionViewModel pvm = new PermissionViewModel()
            {
                PermissionID = permission.PermissionID,
                PermissionName = permission.PermissionName,
                PermissionDescription = permission.PermissionDescription,
                assignedRoles = permission.Roles.ToList()
            };

            return PartialView("_PermissionRoleTable", pvm);

        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeletePermissionRoleReturnPartialView(int PermissionID, int RoleID )
        {
            Role role = database.Roles.Find(RoleID);
            Permission permission = database.Permissions.Find(PermissionID);


            if (permission.Roles.Contains(role))
            {
                permission.Roles.Remove(role);
                database.SaveChanges();
            }

            ViewBag.IsSaved = true;


            permission = database.Permissions.Find(PermissionID);

            PermissionViewModel pvm = new PermissionViewModel()
            {
                PermissionID = permission.PermissionID,
                PermissionName = permission.PermissionName,
                PermissionDescription = permission.PermissionDescription,
                assignedRoles = permission.Roles.ToList()

            };

            return PartialView("_PermissionRoleTable", pvm);
        }

        #endregion

        #region Timers

        [FreezeDown]
        public ActionResult GetTimerList()
        {
            List<LocationDockQuery> ldq = _repo.GetLocationDockings1();


            List<TimerModel> list = new List<TimerModel>();

            foreach (var d in ldq)
            {

                list.Add(new TimerModel
                {
                    TimeDisplayName = "Display_" + d.LocationCode,
                    ReleaseDateTime = d.EndTime.ToString()

                });

            }

            ViewBag.TimerList = list;



            return View("TimerReset", ldq);

        }

        [FreezeDown]

        public ActionResult Reset(int DockingID, int LocationID)
        {


            int UserID = GetLoggedInUID();


            Location loc = _repo.GetLocation(LocationID);
            Docking dock = _repo.GetDockingByID(DockingID);


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

            return Json(Url.Action("GetTimerList", "Admin"));

        }


        #endregion
        private int GetLoggedInUID()
        {
            string UserName = FreezeDown_ExtendedMethods.GetUID();

            int UserID = _repo.GetUserIDByName(UserName);

            return UserID;
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }

    }
}