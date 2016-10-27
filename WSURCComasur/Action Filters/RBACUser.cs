using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSURCComasur.Models;
public class RBACUser
{
    public int User_Id { get; set; }
    public bool IsSysAdmin { get; set; }
    public string Username { get; set; }
    private List<UserRole> Roles = new List<UserRole>();

    public RBACUser(string _username)
    {
        this.Username = _username;
        this.IsSysAdmin = true;
        GetDatabaseUserRolesPermissions();
    }

    private void GetDatabaseUserRolesPermissions()
    {
        using ( WSURCDBEntities _data = new WSURCDBEntities())
        {            
            USERS _user = _data.USERS.Where(u => u.Username == this.Username).FirstOrDefault();
            if (_user != null)
            {
                this.User_Id = _user.User_Id;
                foreach (ROLES _role in _user.ROLES)
                {
                    UserRole _userRole = new UserRole { Role_Id = _role.Role_Id, RoleName = _role.RoleName };
                    foreach (PERMISSIONS _permission in _role.PERMISSIONS)
                    {
                        _userRole.Permissions.Add(new RolePermission { Permission_Id = _permission.Permission_Id, PermissionDescription = _permission.PermissionDescription });
                    }
                    this.Roles.Add(_userRole);

                    if (!this.IsSysAdmin)
                        this.IsSysAdmin =true;
                }
            }
        }
    }

    public bool HasPermission(string requiredPermission)
    {
        bool bFound = false;
        foreach (UserRole role in this.Roles)
        {
            bFound = (role.Permissions.Where(p => p.PermissionDescription.ToLower() == requiredPermission.ToLower()).ToList().Count > 0);
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
    public int Role_Id { get; set; }
    public string RoleName { get; set; }
    public List<RolePermission> Permissions = new List<RolePermission>();
}

public class RolePermission
{
    public int Permission_Id { get; set; }
    public string PermissionDescription { get; set; }
}
