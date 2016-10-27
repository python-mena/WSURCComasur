using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WSURCComasur.Models;
namespace WSURCComasur.Controllers
{
 [RBAC]
    public class AdminController : Controller
    {
        private WSURCDBEntities  database = new WSURCDBEntities();

        #region USERS
        // GET: Admin
        public ActionResult Index()
        {
            return View(database.USERS.Where(r => r.Inactive == false || r.Inactive == null).OrderBy(r => r.Lastname).ThenBy(r => r.Firstname).ToList());
        }

        public ViewResult UserDetails(int id)
        {
            USERS user = database.USERS.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserCreate(USERS user)
        {
            if (user.Username == "" || user.Username == null)
            {
                ModelState.AddModelError(string.Empty, "Username cannot be blank");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    List<string> results = database.Database.SqlQuery<String>(string.Format("SELECT Username FROM USERS WHERE Username = '{0}'", user.Username)).ToList();
                    bool _userExistsInTable = (results.Count > 0);

                    USERS _user = null;
                    if (_userExistsInTable)
                    {
                        _user = database.USERS.Where(p => p.Username == user.Username).FirstOrDefault();
                        if (_user != null)
                        {
                            if (_user.Inactive == false)
                            {
                                ModelState.AddModelError(string.Empty, "USER already exists!");
                            }
                            else
                            {
                                database.Entry(_user).Entity.Inactive = false;
                                database.Entry(_user).Entity.LastModified = System.DateTime.Now;
                                database.Entry(_user).State = EntityState.Modified;
                                database.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        _user = new USERS();
                        _user.Username = user.Username;
                        _user.Lastname = user.Lastname;
                        _user.Firstname = user.Firstname;
                        _user.Title = user.Title;
                        _user.Initial = user.Initial;
                        _user.EMail = user.EMail;

                        if (ModelState.IsValid)
                        {
                            _user.Inactive = false;
                            _user.LastModified = System.DateTime.Now;

                            database.USERS.Add(_user);
                            database.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //return base.ShowError(ex);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult UserEdit(int id)
        {
            USERS user = database.USERS.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult UserEdit(USERS user)
        {
            USERS _user = database.USERS.Where(p => p.User_Id == user.User_Id).FirstOrDefault();
            if (_user != null)
            {
                try
                {
                    database.Entry(_user).CurrentValues.SetValues(user);
                    database.Entry(_user).Entity.LastModified = System.DateTime.Now;
                    database.SaveChanges();
                }
                catch (Exception)
                {
                    
                }                
            }
            return RedirectToAction("UserDetails", new RouteValueDictionary(new { id = user.User_Id }));
        }

        [HttpPost]
        public ActionResult UserDetails(USERS user)
        {
            if (user.Username == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid USER Name");
            }

            if (ModelState.IsValid)
            {
                database.Entry(user).Entity.Inactive = user.Inactive;
                database.Entry(user).Entity.LastModified = System.DateTime.Now;
                database.Entry(user).State = EntityState.Modified;
                database.SaveChanges();
            }      
            return View(user);
        }

        [HttpGet]
        public ActionResult DeleteUserRole(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                database.SaveChanges();
            }
            return RedirectToAction("Details", "USER", new { id = userId });
        }

        [HttpGet]
        public PartialViewResult filter4Users(string _surname)
        {
            return PartialView("_ListUserTable", GetFilteredUserList(_surname));
        }

        [HttpGet]
        public PartialViewResult filterReset()
        {
            return PartialView("_ListUserTable", database.USERS.Where(r => r.Inactive == false || r.Inactive == null).ToList());
        }

        [HttpGet]
        public PartialViewResult DeleteUserReturnPartialView(int userId)
        {
            try
            {
                USERS user = database.USERS.Find(userId);
                if (user != null)
                {            
                    database.Entry(user).Entity.Inactive = true;
                    database.Entry(user).Entity.User_Id = user.User_Id;
                    database.Entry(user).Entity.LastModified = System.DateTime.Now;
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            catch
            {
            }
            return this.filterReset();
        }

        private IEnumerable<USERS> GetFilteredUserList(string _surname)
        {
            IEnumerable<USERS> _ret = null;
            try
            {
                if (string.IsNullOrEmpty(_surname))
                {
                    _ret = database.USERS.Where(r => r.Inactive == false || r.Inactive == null).ToList();
                }
                else
                {
                    _ret = database.USERS.Where(p => p.Lastname == _surname).ToList();
                }
            }
            catch
            {
            }
            return _ret;
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
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.USERS.Find(userId));
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUserRoleReturnPartialView(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (!role.USERS.Contains(user))
            {
                role.USERS.Add(user);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.USERS.Find(userId));
        }

        private void SetViewBagData(int _userId)
        {
            ViewBag.UserId = _userId;
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            ViewBag.RoleId = new SelectList(database.ROLES.OrderBy(p => p.RoleName), "Role_Id", "RoleName");
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

        #region ROLES
        public ActionResult RoleIndex()
        {
            return View(database.ROLES.OrderBy(r => r.RoleDescription).ToList());
        }

        public ViewResult RoleDetails(int id)
        {
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            ROLES role = database.ROLES.Where(r => r.Role_Id == id)
                   .Include(a => a.PERMISSIONS)
                   .Include(a => a.USERS)
                   .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(database.USERS.Where(r => r.Inactive == false || r.Inactive == null), "Id", "Username");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.PERMISSIONS.OrderBy(a => a.PermissionDescription), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(role);
        }

        public ActionResult RoleCreate()
        {
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View();
        }

        [HttpPost]
        public ActionResult RoleCreate(ROLES _role)
        {
            if (_role.RoleDescription == null)
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                           

                database.ROLES.Add(_role);
                database.SaveChanges();
                return RedirectToAction("RoleIndex");
            }
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleEdit(int id)
        {
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();

            ROLES _role = database.ROLES.Where(r => r.Role_Id == id)
                    .Include(a => a.PERMISSIONS)
                    .Include(a => a.USERS)
                    .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(database.USERS.Where(r => r.Inactive == false || r.Inactive == null), "User_Id", "Username");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.PERMISSIONS.OrderBy(a => a.Permission_Id), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(_role);
        }

        [HttpPost]
        public ActionResult RoleEdit(ROLES _role)
        {
            if (string.IsNullOrEmpty(_role.RoleDescription))
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            //EntityState state = database.Entry(_role).State;
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
              
                database.Entry(_role).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("RoleDetails", new RouteValueDictionary(new { id = _role.Role_Id }));
            }
            // USERS combo
            ViewBag.UserId = new SelectList(database.USERS.Where(r => r.Inactive == false || r.Inactive == null), "User_Id", "Username");

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.PERMISSIONS.OrderBy(a => a.Permission_Id), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleDelete(int id)
        {
            ROLES _role = database.ROLES.Find(id);
            if (_role != null)
            {
                _role.USERS.Clear();
                _role.PERMISSIONS.Clear();

                database.Entry(_role).State = EntityState.Deleted;
                database.SaveChanges();
            }
            return RedirectToAction("RoleIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserFromRoleReturnPartialView(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                database.SaveChanges();
            }
            return PartialView("_ListUsersTable4Role", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUser2RoleReturnPartialView(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (!role.USERS.Contains(user))
            {
                role.USERS.Add(user);
                database.SaveChanges();
            }
            return PartialView("_ListUsersTable4Role", role);
        }

        #endregion

        #region PERMISSIONS

        public ViewResult PermissionIndex()
        {
            List<PERMISSIONS> _permissions = database.PERMISSIONS
                               .OrderBy(wn => wn.PermissionDescription)
                               .Include(a => a.ROLES)
                               .ToList();
            return View(_permissions);
        }

        public ViewResult PermissionDetails(int id)
        {
            PERMISSIONS _permission = database.PERMISSIONS.Find(id);
            return View(_permission);
        }

        public ActionResult PermissionCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PermissionCreate(PERMISSIONS _permission)
        {
            if (_permission.PermissionDescription == null)
            {
                ModelState.AddModelError("Permission Description", "Permission Description must be entered");
            }

            if (ModelState.IsValid)
            {
                database.PERMISSIONS.Add(_permission);
                database.SaveChanges();
                return RedirectToAction("PermissionIndex");
            }
            return View(_permission);
        }

        public ActionResult PermissionEdit(int id)
        {
            PERMISSIONS _permission = database.PERMISSIONS.Find(id);
            ViewBag.RoleId = new SelectList(database.ROLES.OrderBy(p => p.RoleDescription), "Role_Id", "RoleDescription");
            return View(_permission);
        }

        [HttpPost]
        public ActionResult PermissionEdit(PERMISSIONS _permission)
        {
            if (ModelState.IsValid)
            {
                database.Entry(_permission).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("PermissionDetails", new RouteValueDictionary(new { id = _permission.Permission_Id }));
            }
            return View(_permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult PermissionDelete(int id)
        {
            PERMISSIONS permission = database.PERMISSIONS.Find(id);
            if (permission.ROLES.Count > 0)
                permission.ROLES.Clear();

            database.Entry(permission).State = EntityState.Deleted;
            database.SaveChanges();
            return RedirectToAction("PermissionIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddPermission2RoleReturnPartialView(int id, int permissionId)
        {
            ROLES role = database.ROLES.Find(id);
            PERMISSIONS _permission = database.PERMISSIONS.Find(permissionId);

            if (!role.PERMISSIONS.Contains(_permission))
            {
                role.PERMISSIONS.Add(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListPermissions", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddAllPermissions2RoleReturnPartialView(int id)
        {
            ROLES _role = database.ROLES.Where(p => p.Role_Id == id).FirstOrDefault();
            List<PERMISSIONS> _permissions = database.PERMISSIONS.ToList();
            foreach (PERMISSIONS _permission in _permissions)
            {
                if (!_role.PERMISSIONS.Contains(_permission))
                {
                    _role.PERMISSIONS.Add(_permission);
                 
                }
            }
            database.SaveChanges();
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeletePermissionFromRoleReturnPartialView(int id, int permissionId)
        {
            ROLES _role = database.ROLES.Find(id);
            PERMISSIONS _permission = database.PERMISSIONS.Find(permissionId);

            if (_role.PERMISSIONS.Contains(_permission))
            {
                _role.PERMISSIONS.Remove(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRoleFromPermissionReturnPartialView(int id, int permissionId)
        {
            ROLES role = database.ROLES.Find(id);
            PERMISSIONS permission = database.PERMISSIONS.Find(permissionId);

            if (role.PERMISSIONS.Contains(permission))
            {
                role.PERMISSIONS.Remove(permission);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Permission", permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRole2PermissionReturnPartialView(int permissionId, int roleId)
        {
            ROLES role = database.ROLES.Find(roleId);
            PERMISSIONS _permission = database.PERMISSIONS.Find(permissionId);

            if (!role.PERMISSIONS.Contains(_permission))
            {
                role.PERMISSIONS.Add(_permission);
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
                    PERMISSIONS _permission = database.PERMISSIONS.Where(p => p.PermissionDescription == _permissionDescription).FirstOrDefault();
                    if (_permission == null)
                    {
                        if (ModelState.IsValid)
                        {
                            PERMISSIONS _perm = new PERMISSIONS();
                            _perm.PermissionDescription = _permissionDescription;

                            database.PERMISSIONS.Add(_perm);
                            database.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("PermissionIndex");
        }
        #endregion
    }
} 

