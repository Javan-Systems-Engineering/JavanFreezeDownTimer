using System;
using System.Collections;
using System.Linq;
using Models;
using Javan.CustomHelpers;
using System.Collections.Generic;
using FreezeDownTimer.Filters;
using System.Data.Entity.SqlServer;
using System.Web.UI.WebControls;
using System.Data.Entity;

namespace FreezeDownTimer.Models
{
    public class DisconnectedRepository
    {
        public int IsValidUser(User user)
        {
            using (var context = new FreezeDownContext())
            {
                var rslt =
                 context.Users
                .Any(u => u.UserName.ToLower() == user
                .UserName.ToLower() && u.Password == user.Password);

                if (rslt == true)
                {
                    return context.Users.Where(u => u.UserName.ToLower() == user.UserName.ToLower() && u.Password == user.Password).FirstOrDefault().UserID;
                }
                else
                {
                    return 0;

                }
            }
        }
        public int GetUserIDByName(string UserName)
        {
            using (var context = new FreezeDownContext())
            {
                var rslt =
                 context.Users
                .Any(u => u.UserName.ToLower() == UserName.ToLower());

                if (rslt == true)
                {
                    return context.Users.Where(u => u.UserName.ToLower() == UserName.ToLower() ).FirstOrDefault().UserID;
                }
                else
                {
                    return 0;

                }
            }
        }


        public IEnumerable GetCartLines()
        {
            using (var context = new FreezeDownContext())
            {

                return context.Carts.AsNoTracking()
                   .Select(c => new { c.LineNumber }).Distinct().OrderBy(c => c.LineNumber).ToList();



            }

        }
        public IEnumerable GetCartNumbers()
        {
            using (var context = new FreezeDownContext())
            {

                return context.Carts.AsNoTracking()
                   .Select(c => new { c.CartNumber }).Distinct().OrderBy(c => c.CartNumber).ToList();
            }

        }

        public IEnumerable GetUsers()
        {
            using (var context = new FreezeDownContext())
            {

                return context.Users.AsNoTracking()
                  .Select(c => new { c.UserName }).Distinct().OrderBy(c => c.UserName).ToList();


            }
        }

        public List<UserViewModel> GetUserList()
        {
            using (var context = new FreezeDownContext())
            {

                var u2 = from u in context.Users.AsNoTracking()
                         select new UserViewModel
                         {
                             UserID = u.UserID,
                             UserName = u.UserName,
                             LastName = u.LastName,
                             FirstName = u.FirstName,
                             Password = u.Password,
                             Active = (bool) !(u.Inactive.HasValue ? u.Inactive : false)

                         };

                return u2.ToList();
            }

        }

        public List<RoleViewModel> GetRoleList()
        {
            using (var context = new FreezeDownContext())
            {

                var r2 = from r in context.Roles.AsNoTracking()
                         select new RoleViewModel
                         {
                             RoleID = r.RoleID,
                             RoleName = r.RoleName,
                             RoleDescription = r.RoleDescription,
                             IsSysAdmin = r.IsSysAdmin,

                         };

                return r2.ToList();
            }

        }


        public List<PermissionViewModel> GetPermissionList()
        {
            using (var context = new FreezeDownContext())
            {

                var p2 = from p in context.Permissions.AsNoTracking()
                         select new PermissionViewModel
                         {
                             PermissionID = p.PermissionID,
                             PermissionName = p.PermissionName,
                             PermissionDescription = p.PermissionDescription

                         };

                return p2.ToList();
            }

        }

        public IEnumerable GetUsersForDD()
        {
            using (var context = new FreezeDownContext())
            {

                var uq = context.Users.AsNoTracking().Where(c => c.Inactive == false)
                  .Select(c => new { c.UserName }).Distinct().OrderBy(c => c.UserName);

                List<string> lp = new List<string>();

                foreach (var u in uq)
                {
                    FreezeDownUser fdu = new FreezeDownUser(u.UserName);
                    bool HasPermission = fdu.HasPermission("timer-StartFreeze1");
                    bool IsAdmin = fdu.IsSysAdmin;
                    if (HasPermission == false && IsAdmin == false)
                    {
                        lp.Add(u.UserName);
                    }
                }

                return uq.Where(u => !lp.Contains(u.UserName)).ToList(); 
            }
        }

        public bool IsUserViewOnly(string username)
        {

            return false;
        }

        public bool IsCartInUse(string LineNumber, int CartNumber)
        {
            using (var context = new FreezeDownContext())
            {
                var cq = (from c in context.Carts.AsNoTracking()
                          join d in context.Dockings.AsNoTracking() on c.CartID equals d.CartID into dt
                          from d in dt.DefaultIfEmpty()
                          where ((c.LineNumber == LineNumber) && (c.CartNumber == CartNumber) && (d.IsActive.HasValue && d.IsActive == true ))
                          select c);

                if (cq.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

             }


        }

        public bool IsCartValid(string LineNumber, int CartNumber)
        {
            using (var context = new FreezeDownContext())
            {
                var cq = (from c in context.Carts.AsNoTracking()
                          where ((c.LineNumber == LineNumber) && (c.CartNumber == CartNumber) )
                          select c);

                if (cq.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }


        }

        public int GetCartID(string lineNumber, int cartNumber )
        {
            using (var context = new FreezeDownContext())
            {

                return context.Carts.AsNoTracking().Where(c => (c.LineNumber == lineNumber) && (c.CartNumber == cartNumber))
                   .Select(c => c ).FirstOrDefault().CartID;
            }

        }
        public int GetLocationID(string LocationCode)
        {
            using (var context = new FreezeDownContext())
            {

                return context.Locations.AsNoTracking().Where(c => c.LocationCode == LocationCode)
                   .Select(c => c).FirstOrDefault().LocationID;
            }

        }

        public Location GetLocation(int LocationID)
        {
            using (var context = new FreezeDownContext())
            {

                return context.Locations.AsNoTracking().Where(c => c.LocationID == LocationID)
                   .Select(c => c).FirstOrDefault();
            }

        }
        public bool CreateDocking(Docking docking)
        {
            bool rslt = false;

            using (var context = new FreezeDownContext())
            {

                Location loc = context.Locations.AsNoTracking().FirstOrDefault(i => i.LocationID == docking.LocationID);
                if (loc.IsOcuppied == false)
                {
                    loc.IsOcuppied = true;
                    loc.LastUpdate = DateTime.Now.ConvertToEST();
                }
                else 
                {
                    return rslt;
                }

                context.Entry(loc).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                context.Dockings.Add(docking);
                context.SaveChanges();


                rslt = true;

            }

            return rslt;
        }

        public List<LocationDockQuery> GetLocationDockings()
        {
            using (var context = new FreezeDownContext())
            {
                var dck = from d in context.Dockings.AsNoTracking()
                          join l in context.Locations.AsNoTracking() on d.LocationID equals l.LocationID
                          join c in context.Carts.AsNoTracking() on d.CartID equals c.CartID
                          where (d.IsActive == true && l.IsOcuppied == true)
                          select new LocationDockQuery
                          {
                              DockingID = d.DockingID,
                              EndTime = d.EndTime ?? DateTime.MinValue,
                             LocationID  = l.LocationID,
                             LocationCode = l.LocationCode,
                             AssociatedLocationCode = l.AssociatedLocationCode,
                             LineNumber = c.LineNumber,
                             CartNumber = c.CartNumber ?? default(int)

            };

                return dck.ToList();
            }

        }

        public List<LocationDockQuery> GetLocationDockings1()
        {
            using (var context = new FreezeDownContext())
            {
                var dck = (from d in context.Dockings.AsNoTracking()
                           join l in context.Locations.AsNoTracking() on d.LocationID equals l.LocationID
                           join c in context.Carts.AsNoTracking() on d.CartID equals c.CartID
                           where (d.IsActive == true && l.IsOcuppied == true)
                           select new LocationDockQuery
                           {
                               DockingID = d.DockingID,
                               EndTime = d.EndTime ?? DateTime.MinValue,
                               LocationID = l.LocationID,
                               LocationCode = l.LocationCode,
                               AssociatedLocationCode = l.AssociatedLocationCode,
                               LineNumber = c.LineNumber,
                               CartNumber = c.CartNumber ?? default(int)

                           })
                          .AsEnumerable()
                          .Select(x => new LocationDockQuery
                          {
                              DockingID = x.DockingID,
                              EndTime = x.EndTime,
                              RemainingTime = (x.EndTime > DateTime.Now.ConvertToEST() ? x.EndTime - DateTime.Now.ConvertToEST() : new TimeSpan(0,0,0)).ToString(@"dd\.hh\:mm\:ss"),
                              LocationID = x.LocationID,
                              LocationCode = x.LocationCode,
                              AssociatedLocationCode = x.AssociatedLocationCode,
                              FullLocationCode = x.AssociatedLocationCode + " - " + x.LocationCode,
                              LineNumber = x.LineNumber,
                              CartNumber = x.CartNumber 

                          });

                return dck.ToList();
            }

        }

        public Docking  GetDocking(int CartID, int LocationID)
        {
            using (var context = new FreezeDownContext())
            {

                return context.Dockings.AsNoTracking().Where(c => c.LocationID == LocationID && c.CartID == CartID && c.IsActive == true)
                   .Select(c => c).FirstOrDefault();
            }



        }
        
        public Docking GetDockingByID(int DockingID)
        {
            using (var context = new FreezeDownContext())
            {

                return context.Dockings.AsNoTracking().Where(c => c.DockingID == DockingID  && c.IsActive == true)
                   .Select(c => c).FirstOrDefault();
            }



        }

        public bool UpdateLocationDock(Location loc, Docking dock )
        {
            bool rslt = false;

            using (var context = new FreezeDownContext())
            {

                context.Entry(loc).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                context.Entry(dock).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                rslt = true;
            }

            return rslt;

        }

        public List<AuditReportModel> GetAuditReport(string StartTime, string EndTime  )
        {

            using (var context = new FreezeDownContext())
            {

                var ar = from d in context.Dockings.AsNoTracking()
                join l in context.Locations.AsNoTracking() on d.LocationID equals l.LocationID into lt
                from l in lt.DefaultIfEmpty()
                join c in context.Carts.AsNoTracking() on d.CartID equals c.CartID into ct
                from c in ct.DefaultIfEmpty()
                join u in context.Users.AsNoTracking() on d.LastUpdateBy equals u.UserID into ut
                from u in ut.DefaultIfEmpty()
                select new AuditReportModel
                {
                    UserName = u.UserName,
                    StartTime = d.StartTime ?? DateTime.MinValue,
                    EndTime = d.EndTime ?? DateTime.MinValue,
                    LocationCode = l.LocationCode,
                    Cart = c.LineNumber + "-" + SqlFunctions.Replicate("0",  2 - c.CartNumber.ToString().Length) + c.CartNumber.ToString(),
                    //CartRemoved = d.LastUpdate ?? DateTime.MinValue
                    CartRemoved = d.LastUpdate 

                };

                if (String.IsNullOrEmpty(StartTime) == false  && String.IsNullOrEmpty(EndTime) == false)
                {

                   DateTime dtStartTime = Convert.ToDateTime(StartTime);
                   DateTime dtEndTime = Convert.ToDateTime(EndTime);
                   ar = ar.Where(c => (c.StartTime > dtStartTime) && (c.EndTime < dtEndTime));
                }


                return ar.ToList();
                    
            }

            

        }

        public User GetUserByID(int UserID)
        {

            using (var context = new FreezeDownContext())
            {

                var user = context.Users.Include(u => u.Roles).Where(u => u.UserID == UserID).FirstOrDefault();

                //var user = user.Roles.

                return user;

            }

        }

        public Role GetRoleByID(int RoleID)
        {

            using (var context = new FreezeDownContext())
            {

                var role = context.Roles.Include(u => u.Users).Include(u => u.Permissions).Where(u => u.RoleID == RoleID).FirstOrDefault();

                return role;

            }

        }

        public Permission GetPermissionByID(int PermissionID)
        {

            using (var context = new FreezeDownContext())
            {

                var permission = context.Permissions.Include(u => u.Roles).Where(u => u.PermissionID == PermissionID).FirstOrDefault();

                return permission;

            }

        }

        public User GetUserByID2(int UserID)
        {

            using (var context = new FreezeDownContext())
            {

                var user = context.Users.Find(UserID);

                return user;

            }

        }

        public Role GetRoleByID2(int RoleID)
        {

            using (var context = new FreezeDownContext())
            {

                var role = context.Roles.Find(RoleID);

                return role;

            }

        }
        public IEnumerable GetRoles()
        {
            using (var context = new FreezeDownContext())
            {
                return context.Roles.AsNoTracking().OrderBy(c => c.RoleName)
                    .Select(c => new { c.RoleID, c.RoleName}).ToList();

            }

        }

        public IEnumerable GetPermissions()
        {
            using (var context = new FreezeDownContext())
            {
                return context.Permissions.AsNoTracking().OrderBy(c => c.PermissionName)
                    .Select(c => new { c.PermissionID, c.PermissionName }).ToList();

            }

        }
        public bool UpdateUser(User user)
        {
            using (var context = new FreezeDownContext())
            {
                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }

        public bool AddUserRole(int RoleID, int UserID)
        {
            using (var context = new FreezeDownContext())
            {
                Role role = context.Roles.Find(RoleID);
                User user = context.Users.Find(UserID);

                role.Users.Add(user);

                //context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }


        public bool DeleteUserRole(int RoleID, int UserID)
        {
            using (var context = new FreezeDownContext())
            {
                Role role = context.Roles.Find(RoleID);
                User user = context.Users.Find(UserID);

                role.Users.Remove(user);
                context.SaveChanges();

            }

            return true;
        }

        public bool UpdateRole(Role role)
        {
            using (var context = new FreezeDownContext())
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }

        public bool AddRolePermission(int RoleID, int PermissionID)
        {
            using (var context = new FreezeDownContext())
            {
                Role role = context.Roles.Find(RoleID);
                Permission permission = context.Permissions.Find(PermissionID);

                role.Permissions.Add(permission);

                //context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }


        public bool DeleteRolePermission(int RoleID, int PermissionID)
        {
            using (var context = new FreezeDownContext())
            {
                Role role = context.Roles.Find(RoleID);
                Permission permission = context.Permissions.Find(PermissionID);

                role.Permissions.Remove(permission);
                context.SaveChanges();

            }

            return true;
        }

        public bool AddPermissionRole(int PermissionID, int RoleID)
        {
            using (var context = new FreezeDownContext())
            {
                Role role = context.Roles.Find(RoleID);
                Permission permission = context.Permissions.Find(PermissionID);

                permission.Roles.Add(role);

                //context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }
        public bool UpdatePermission(Permission permission)
        {
            using (var context = new FreezeDownContext())
            {
                context.Entry(permission).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }

    }
}