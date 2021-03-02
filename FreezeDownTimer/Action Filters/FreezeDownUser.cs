using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FreezeDownTimer.Filters
{
    public class FreezeDownUser
    {
        public int UserID { get; set; }
        public bool IsSysAdmin { get; set; }
        public string UserName { get; set; }
        private List<UserRole> Roles = new List<UserRole>();

        public FreezeDownUser(string _username)
        {
            this.UserName = _username;
            this.IsSysAdmin = false;
            GetDatabaseUserRolesPermissions();
        }

        private void GetDatabaseUserRolesPermissions()
        {
            using (FreezeDownContext _data = new FreezeDownContext())
            {
                User _user = _data.Users.Where(u => u.UserName == this.UserName).FirstOrDefault();
                if (_user != null)
                {
                    this.UserID = _user.UserID;
                    foreach (Role _role in _user.Roles)
                    {
                        UserRole _userRole = new UserRole { RoleID = _role.RoleID, RoleName = _role.RoleName };
                        foreach (Permission _permission in _role.Permissions)
                        {
                            _userRole.Permissions.Add(new RolePermission { PermissionID = _permission.PermissionID, PermissionName = _permission.PermissionName });
                        }
                        this.Roles.Add(_userRole);

                        if (!this.IsSysAdmin)
                            this.IsSysAdmin = _role.IsSysAdmin;
                    }
                }
            }
        }

        public bool HasPermission(string requiredPermission)
        {
            bool bFound = false;
            foreach (UserRole role in this.Roles)
            {
                bFound = (role.Permissions.Where(p => p.PermissionName.ToLower() == requiredPermission.ToLower()).ToList().Count > 0);
                if (bFound)
                    break;
            }
            return bFound;
        }

        public bool HasAdminPermission()
        {
            bool bFound = false;
            foreach (UserRole role in this.Roles)
            {
                bFound = (role.Permissions.Where(p => p.PermissionName.ToLower().Contains("admin")).ToList().Count > 0);
                if (bFound)
                    break;
            }
            return bFound;
        }



        public bool HasRole(string role)
        {
            return (Roles.Where(p => p.RoleName == role).ToList().Count > 0);
        }

        public bool HasRoles(string roles)
        {
            bool bFound = false;
            string[] _roles = roles.ToLower().Split(';');
            foreach (UserRole role in this.Roles)
            {
                try
                {
                    bFound = _roles.Contains(role.RoleName.ToLower());
                    if (bFound)
                        return bFound;
                }
                catch (Exception)
                {
                }
            }
            return bFound;
        }
    }

    public class UserRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public List<RolePermission> Permissions = new List<RolePermission>();
    }

    public class RolePermission
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
    }

}