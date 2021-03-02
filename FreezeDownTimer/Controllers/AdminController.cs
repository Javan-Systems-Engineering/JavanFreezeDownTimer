using FreezeDownTimer.Filters;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using PagedList;
using Javan.CustomHelpers;
using FreezeDownTimer.Models;

namespace FreezeDownTimer.Controllers
{
    [FreezeDown]
    public class AdminController : Controller
    {
        private FreezeDownContext database = new FreezeDownContext();

        private readonly DisconnectedRepository _repo = new DisconnectedRepository();




        #region Users
        // GET: Admin
        [FreezeDown]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string subReset, int? page)
        {
            try
            {

                ViewBag.CurrentSort = sortOrder;
                ViewBag.FirstNameSortParm = sortOrder == "firstname" ? "firstname_desc" : "firstname";
                ViewBag.LastNameSortParm = sortOrder == "lastname" ? "lastname_desc" : "lastname";
                ViewBag.UserNameSortParm = sortOrder == "username" ? "username_desc" : "username";

                if (subReset == "Reset")
                {
                    currentFilter = "";
                    searchString = "";
                    ModelState.Clear();
                }

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var users = database.Users.Where(r => r.Inactive == false || r.Inactive == null);


                if (!String.IsNullOrEmpty(searchString))
                {
                    users = users.Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "firstname_desc":
                        users = users.OrderByDescending(u => u.FirstName);
                        break;
                    case "firstname":
                        users = users.OrderBy(u => u.FirstName);
                        break;

                    case "lastname_desc":
                        users = users.OrderByDescending(u => u.LastName);
                        break;
                    case "lastname":
                        users = users.OrderBy(u => u.LastName);
                        break;

                    case "username_desc":
                        users = users.OrderByDescending(u => u.UserName);
                        break;
                    case "username":
                        users = users.OrderBy(u => u.UserName);
                        break;

                    default:
                        users = users.OrderBy(u => u.LastName);
                        break;
                }

                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(users.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }

        public ActionResult UserDetails(int id)
        {
            try
            {
                User user = database.Users.Find(id);
                SetViewBagData(id);
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        [FreezeDown]
        public ActionResult UserCreate(User user)
        {
            try
            {

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
                                    return RedirectToAction("Index");
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
                                return RedirectToAction("Index");
                            }
                        }

                    }


                return View(user);
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }

        }
        [HttpGet]
        public ActionResult UserEdit(int id)
        {
            User user = database.Users.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult UserEdit(User user)
        {
            try
            {
                User _user = database.Users.Where(p => p.UserID == user.UserID).FirstOrDefault();
                if (_user != null)
                {

                        database.Entry(_user).CurrentValues.SetValues(user);
                        database.Entry(_user).Entity.LastUpdate = DateTime.Now.ConvertToEST();
                        database.SaveChanges();

                }
                return RedirectToAction("UserDetails", new RouteValueDictionary(new { id = user.UserID }));
            }
            catch (Exception ex)
            {
                TempData["FreezeDownError"] = ex;
                return RedirectToAction("ShowError", "Error");
            }
        }

        [HttpPost]
        public ActionResult UserDetails(User user)
        {
            if (user.UserName == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name");
            }

            if (ModelState.IsValid)
            {
                database.Entry(user).Entity.Inactive = user.Inactive;
                database.Entry(user).Entity.LastUpdate = DateTime.Now.ConvertToEST();
                database.Entry(user).State = EntityState.Modified;
                database.SaveChanges();
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult DeleteUserRole(int id, int userId)
        {
            Role role = database.Roles.Find(id);
            User user = database.Users.Find(userId);

            if (role.Users.Contains(user))
            {
                role.Users.Remove(user);
                database.SaveChanges();
            }
            return RedirectToAction("Details", "User", new { id = userId });
        }

        [HttpGet]
        public PartialViewResult filter4Users(string _surname, int _page, string _sortOrder)
        {
            //return PartialView("_ListUserTable", GetFilteredUserList(_surname));
             return PartialView("_ListUserTable", GetPagedFilteredUserList(_surname, _page, _sortOrder));

        }

        [HttpGet]
        public PartialViewResult filterReset()
        {
            return PartialView("_ListUserTable", database.Users.Where(r => r.Inactive == false || r.Inactive == null).ToList());
        }

        [HttpGet]
        public PartialViewResult DeleteUserReturnPartialView(int userId)
        {
            try
            {
                User user = database.Users.Find(userId);
                if (user != null)
                {
                    database.Entry(user).Entity.Inactive = true;
                    database.Entry(user).Entity.UserID = user.UserID;
                    database.Entry(user).Entity.LastUpdate = DateTime.Now.ConvertToEST();
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            catch
            {
            }
            return this.filterReset();
        }

        private IEnumerable<User> GetFilteredUserList(string _surname)
        {
            IEnumerable<User> _ret = null;
            try
            {
                if (string.IsNullOrEmpty(_surname))
                {
                    _ret = database.Users.Where(r => r.Inactive == false || r.Inactive == null).ToList();
                }
                else
                {
                    _ret = database.Users.Where(p => p.LastName == _surname).ToList();
                }
            }
            catch
            {
            }
            return _ret;
        }

        private PagedList.IPagedList<User> GetPagedFilteredUserList(string _surname, int? _page, string _sortOrder)
        {
            ViewBag.CurrentSort = _sortOrder;

            var Users =
                from u in database.Users.Where(r => r.Inactive == false || r.Inactive == null)
                select u;

            try
            {

                if (!string.IsNullOrEmpty(_surname))
                {
                    Users = Users.Where(p => p.LastName.Contains(_surname));
                }
                switch (_sortOrder)
                {
                    case "firstname_desc":
                        Users = Users.OrderByDescending(u => u.FirstName);
                        break;
                    case "firstname":
                        Users = Users.OrderBy(u => u.FirstName);
                        break;

                    case "lastname_desc":
                        Users = Users.OrderByDescending(u => u.LastName);
                        break;
                    case "lastname":
                        Users = Users.OrderBy(u => u.LastName);
                        break;

                    case "username_desc":
                        Users = Users.OrderByDescending(u => u.UserName);
                        break;
                    case "username":
                        Users = Users.OrderBy(u => u.UserName);
                        break;

                    default:
                        Users = Users.OrderBy(u => u.LastName);
                        break;
                }
            }


            catch
            {
            }
            int pageSize = 5;
            int pageNumber = (_page ?? 1);

            return Users.ToPagedList(pageNumber, pageSize); 

        }
        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserRoleReturnPartialView(int id, int userId)
        {
            Role role = database.Roles.Find(id);
            User user = database.Users.Find(userId);

            if (role.Users.Contains(user))
            {
                role.Users.Remove(user);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.Users.Find(userId));
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUserRoleReturnPartialView(int id, int userId)
        {
            Role role = database.Roles.Find(id);
            User user = database.Users.Find(userId);

            if (!role.Users.Contains(user))
            {
                role.Users.Add(user);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.Users.Find(userId));
        }

        private void SetViewBagData(int _userId)
        {
            ViewBag.UserID = _userId;
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            ViewBag.RoleID = new SelectList(database.Roles.OrderBy(p => p.RoleName), "RoleID", "RoleName");
        }

        public List<SelectListItem> List_boolNullYesNo()
        {
            var _retVal = new List<SelectListItem>();
            try
            {
                _retVal.Add(new SelectListItem { Text = "Not Set", Value = null });
                _retVal.Add(new SelectListItem { Text = "Yes", Value = bool.TrueString });
                _retVal.Add(new SelectListItem { Text = "No", Value = bool.FalseString });
            }
            catch { }
            return _retVal;
        }
        #endregion

        #region Roles
        public ActionResult RoleIndex()
        {
            return View(database.Roles.OrderBy(r => r.RoleDescription).ToList());
        }

        public ViewResult RoleDetails(int id)
        {
            User user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            Role role = database.Roles.Where(r => r.RoleID == id)
                   .Include(a => a.Permissions)
                   .Include(a => a.Users)
                   .FirstOrDefault();

            // Users combo
            ViewBag.UserId = new SelectList(database.Users.Where(r => r.Inactive == false || r.Inactive == null), "Id", "UserName");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.Permissions.OrderBy(a => a.PermissionDescription), "PermissionID", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(role);
        }

        public ActionResult RoleCreate()
        {
            User user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View();
        }

        [HttpPost]
        public ActionResult RoleCreate(Role _role)
        {
            if (_role.RoleDescription == null)
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            User user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                _role.InsertDate = DateTime.Now.ConvertToEST();
                _role.LastUpdate = DateTime.Now.ConvertToEST();
                _role.LastUpdateBy = user.UserID;
                database.Roles.Add(_role);
                database.SaveChanges();
                return RedirectToAction("RoleIndex");
            }
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleEdit(int id)
        {
            User user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();

            Role _role = database.Roles.Where(r => r.RoleID == id)
                    .Include(a => a.Permissions)
                    .Include(a => a.Users)
                    .FirstOrDefault();

            // Users combo
            ViewBag.UserId = new SelectList(database.Users.Where(r => r.Inactive == false || r.Inactive == null), "UserID", "UserName");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.Permissions.OrderBy(a => a.PermissionID), "PermissionID", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(_role);
        }

        [HttpPost]
        public ActionResult RoleEdit(Role _role)
        {
            if (string.IsNullOrEmpty(_role.RoleDescription))
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            //EntityState state = database.Entry(_role).State;
            User user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                _role.LastUpdate = DateTime.Now.ConvertToEST();
                database.Entry(_role).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("RoleDetails", new RouteValueDictionary(new { id = _role.RoleID }));
            }
            // Users combo
            ViewBag.UserId = new SelectList(database.Users.Where(r => r.Inactive == false || r.Inactive == null), "UserID", "UserName");

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.Permissions.OrderBy(a => a.PermissionID), "PermissionID", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleDelete(int id)
        {
            Role _role = database.Roles.Find(id);
            if (_role != null)
            {
                _role.Users.Clear();
                _role.Permissions.Clear();

                database.Entry(_role).State = EntityState.Deleted;
                database.SaveChanges();
            }
            return RedirectToAction("RoleIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserFromRoleReturnPartialView(int id, int userId)
        {
            Role role = database.Roles.Find(id);
            User user = database.Users.Find(userId);

            if (role.Users.Contains(user))
            {
                role.Users.Remove(user);
                database.SaveChanges();
            }
            return PartialView("_ListUsersTable4Role", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUser2RoleReturnPartialView(int id, int userId)
        {
            Role role = database.Roles.Find(id);
            User user = database.Users.Find(userId);

            if (!role.Users.Contains(user))
            {
                role.Users.Add(user);
                database.SaveChanges();
            }
            return PartialView("_ListUsersTable4Role", role);
        }

        #endregion

        #region Permissions

        public ViewResult PermissionIndex()
        {
            List<Permission> _permissions = database.Permissions
                               .OrderBy(wn => wn.PermissionDescription)
                               .Include(a => a.Roles)
                               .ToList();
            return View(_permissions);
        }

        public ViewResult PermissionDetails(int id)
        {
            Permission _permission = database.Permissions.Find(id);
            return View(_permission);
        }

        public ActionResult PermissionCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PermissionCreate(Permission _permission)
        {
            if (_permission.PermissionDescription == null)
            {
                ModelState.AddModelError("Permission Description", "Permission Description must be entered");
            }

            if (ModelState.IsValid)
            {
                _permission.InsertDate = DateTime.Now.ConvertToEST();
                _permission.LastUpdate = DateTime.Now.ConvertToEST();
                database.Permissions.Add(_permission);
                database.SaveChanges();
                return RedirectToAction("PermissionIndex");
            }
            return View(_permission);
        }

        public ActionResult PermissionEdit(int id)
        {
            Permission _permission = database.Permissions.Find(id);
            ViewBag.RoleId = new SelectList(database.Roles.OrderBy(p => p.RoleDescription), "RoleID", "RoleDescription");
            return View(_permission);
        }

        [HttpPost]
        public ActionResult PermissionEdit(Permission _permission)
        {
            if (ModelState.IsValid)
            {
                _permission.LastUpdate = DateTime.Now.ConvertToEST();
                database.Entry(_permission).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("PermissionDetails", new RouteValueDictionary(new { id = _permission.PermissionID }));
            }
            return View(_permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult PermissionDelete(int id)
        {
            Permission permission = database.Permissions.Find(id);
            if (permission.Roles.Count > 0)
                permission.Roles.Clear();

            database.Entry(permission).State = EntityState.Deleted;
            database.SaveChanges();
            return RedirectToAction("PermissionIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddPermission2RoleReturnPartialView(int id, int permissionId)
        {
            Role role = database.Roles.Find(id);
            Permission _permission = database.Permissions.Find(permissionId);

            if (!role.Permissions.Contains(_permission))
            {
                role.Permissions.Add(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListPermissions", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddAllPermissions2RoleReturnPartialView(int id)
        {
            Role _role = database.Roles.Where(p => p.RoleID == id).FirstOrDefault();
            List<Permission> _permissions = database.Permissions.ToList();
            foreach (Permission _permission in _permissions)
            {
                if (!_role.Permissions.Contains(_permission))
                {
                    _role.Permissions.Add(_permission);

                }
            }
            database.SaveChanges();
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeletePermissionFromRoleReturnPartialView(int id, int permissionId)
        {
            Role _role = database.Roles.Find(id);
            Permission _permission = database.Permissions.Find(permissionId);

            if (_role.Permissions.Contains(_permission))
            {
                _role.Permissions.Remove(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRoleFromPermissionReturnPartialView(int id, int permissionId)
        {
            Role role = database.Roles.Find(id);
            Permission permission = database.Permissions.Find(permissionId);

            if (role.Permissions.Contains(permission))
            {
                role.Permissions.Remove(permission);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Permission", permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRole2PermissionReturnPartialView(int permissionId, int roleId)
        {
            Role role = database.Roles.Find(roleId);
            Permission _permission = database.Permissions.Find(permissionId);

            if (!role.Permissions.Contains(_permission))
            {
                role.Permissions.Add(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Permission", _permission);
        }

        public ActionResult PermissionsImport()
        {
            var _controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t != null
                    && t.IsPublic
                    && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                    && !t.IsAbstract
                    && typeof(IController).IsAssignableFrom(t));

            var _controllerMethods = _controllerTypes.ToDictionary(controllerType => controllerType,
                    controllerType => controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType)));

            foreach (var _controller in _controllerMethods)
            {
                string _controllerName = _controller.Key.Name;
                foreach (var _controllerAction in _controller.Value)
                {
                    string _controllerActionName = _controllerAction.Name;
                    if (_controllerName.EndsWith("Controller"))
                    {
                        _controllerName = _controllerName.Substring(0, _controllerName.LastIndexOf("Controller"));
                    }

                    string _permissionDescription = string.Format("{0}-{1}", _controllerName, _controllerActionName);
                    Permission _permission = database.Permissions.Where(p => p.PermissionDescription == _permissionDescription).FirstOrDefault();
                    if (_permission == null)
                    {
                        if (ModelState.IsValid)
                        {
                            Permission _perm = new Permission();
                            _perm.PermissionDescription = _permissionDescription;

                            database.Permissions.Add(_perm);
                            database.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("PermissionIndex");
        }
        #endregion

        #region Timers
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

    }
}