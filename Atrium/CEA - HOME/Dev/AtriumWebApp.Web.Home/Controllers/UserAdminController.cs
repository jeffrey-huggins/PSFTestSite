using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Home.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Home.Controllers
{
    [RestrictAccessWithApp(SysAdmin = true, RedirectUrl = "Unauthorized")]
    public class UserAdminController : BaseAdminController
    {
        public UserAdminController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        //
        // GET: /UserAdmin/

        public ActionResult Index(int? id)
        {
            AdminUserAppListViewModel userAdminView = new AdminUserAppListViewModel()
            {
                UserList = Context.MasterBusinessUsers.Where(a => a.AccountName != null).ToList(),
                ApplicationList = Context.Applications.Where(a => a.ApplicationId > 0).OrderBy(a => a.ApplicationGroupId).ToList(),
                CommunityList = Context.Facilities.Where(a => a.CommunityId > 0).ToList(),
                ObjectAccessList = Context.ObjectPermission.Where(a => a.ApplicationId != 0).ToList(),
                SelectedUser = id
            };

            return View(userAdminView);
        }

        [HttpGet]
        public ActionResult Navigation()
        {
            Navigation globalNav;
            Session.TryGetObject("globalNav", out globalNav);
            return View(globalNav.Application);
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public ActionResult EditAppInfo(int id)
        {
            Navigation globalNav;
            Session.TryGetObject("globalNav", out globalNav);
            AdminAppInfoViewModel app = new AdminAppInfoViewModel
            {
                AppInfo = Context.Applications.Find(id),
                Groups = globalNav.Groups
            };
            return PartialView("Edit/AppInfo", app);
        }
        [HttpPost]
        public ActionResult EditAppInfo([FromBody]AdminAppInfoViewModel application)
        {
            Context.Applications.Attach(application.AppInfo);
            Context.Entry(application.AppInfo).State = EntityState.Modified;

            JsonResult result;// = new JsonResult();
            if (ModelState.IsValid)
            {
                Context.SaveChanges();
                result = Json(new
                {
                    success = true
                });
            }
            else
            {
                result = Json(new
                {
                    success = false,
                    message = "Please verify all fields are correct."
                });
            }
            return result;
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public JsonResult GetUserAppAccess(int id)
        {
            MasterBusinessUser user = Context.MasterBusinessUsers.Find(id);
            List<int> appIdList = new List<int>();
            foreach (var access in user.UserAccess.Where(a => a.AppFlg || a.ReportFlg))
            {
                if (appIdList.Contains(access.ApplicationId))
                {
                    continue;
                }

                appIdList.Add(access.ApplicationId);
            }

            //foreach(var appInfo)
            return new JsonResult(appIdList);
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public JsonResult GetUserObjectAccess(int userId)
        {
            MasterBusinessUser user = Context.MasterBusinessUsers.Find(userId);
            List<int> accessIdList = new List<int>();
            foreach (var access in user.SpecialAccess)//.Where(a => a.EnabledFlg))
            {
                if (accessIdList.Contains(access.ObjectPermissionId))
                {
                    continue;
                }

                accessIdList.Add(access.ObjectPermissionId);
            }
            return new JsonResult(accessIdList);
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public JsonResult GetAdminAccess(int userId)
        {
            MasterBusinessUser user = Context.MasterBusinessUsers.Find(userId);
            List<int> appAdmin = new List<int>();
            foreach (var access in user.AdminAccess.Where(a => a.AdminFlg))
            {
                appAdmin.Add(access.ApplicationId);
            }
            return new JsonResult(appAdmin);
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public JsonResult GetUserObjectCommunityAccess(int userId, int objectId)
        {
            MasterBusinessUser user = Context.MasterBusinessUsers.Find(userId);
            List<ObjectCommunityAccess> accessList = new List<ObjectCommunityAccess>();
            foreach (var access in user.SpecialAccess.Where(a => a.ObjectPermissionId == objectId))
            {
                ObjectCommunityAccess oca = new ObjectCommunityAccess()
                {
                    ObjectPermissionId = access.ObjectPermissionId,
                    CommunityId = access.CommunityId,
                    EnabledFlag = access.EnabledFlg,
                    RowExists = true
                };
                accessList.Add(oca);
            }
            return new JsonResult(accessList);
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public JsonResult GetUserCommunityAccess(int userId, int appId)
        {
            //MasterBusinessUser user = Context.MasterBusinessUsers.Find(userId);
            //var specialPermissions = user.SpecialAccess.Where(a => a.ObjectPermissions.ApplicationId == appId && a.EnabledFlg);

            List<CommunityAccess> communityIdList = new List<CommunityAccess>();
            foreach (var access in Context.ApplicationUserAccess.Where(a => a.BusinessUserId == userId && a.ApplicationId == appId).ToList())
            {
                CommunityAccess ca = new CommunityAccess()
                {
                    AppFlg = access.AppFlg,
                    CommunityId = access.CommunityId,
                    ReportFlg = access.ReportFlg,
                    AppId = appId,
                    RowExists = true
                };

                communityIdList.Add(ca);
            }

            //foreach(var appInfo)
            return new JsonResult(communityIdList);
        }
        [HttpPost]
        public JsonResult UpdateUserAdminAccess(int userId, int appId, bool isAdmin)
        {
            var user = Context.MasterBusinessUsers.Find(userId);
            var adminAccess = user.AdminAccess.FirstOrDefault(a => a.ApplicationId == appId);
            if(adminAccess == null)
            {
                adminAccess = new SystemAppAdmin()
                {
                    BusinessUserId = userId,
                    ApplicationId = appId,
                    AdminFlg = isAdmin
                };
                Context.SystemAppAdmin.Add(adminAccess);
            }
            else
            {
                adminAccess.AdminFlg = isAdmin;
            }
            Context.SaveChanges();
            return Json(new { Success = true });
        }

        public JsonResult DeleteUserAdminAccess(int userId, int appId)
        {
            var user = Context.MasterBusinessUsers.Find(userId);
            var adminAccess = user.AdminAccess.FirstOrDefault(a => a.ApplicationId == appId);
            if (adminAccess != null)
            {
                Context.SystemAppAdmin.Remove(adminAccess);
                Context.SaveChanges();
            }
            
            return Json(new { Success = true });
        }

        [HttpPost]
        public JsonResult UpdateUserObjectAccess(int userId, int objectId, int communityId, bool accessFlag)
        {
            var user = Context.MasterBusinessUsers.Find(userId);
            var objectItem = Context.ObjectPermission.Find(objectId);
            var objectAccess = objectItem.UserPermissions.FirstOrDefault(a => a.BusinessUserId == userId && a.CommunityId == communityId);
            if (objectAccess == null)
            {
                objectAccess = new SystemObjectPermissionRef()
                {
                    ApplicationId = objectItem.ApplicationId,
                    BusinessUserId = userId,
                    CommunityId = communityId,
                    EnabledFlg = accessFlag,
                    ObjectPermissionId = objectId
                };
                Context.ObjectUserPermission.Add(objectAccess);
                Context.SaveChanges();
            }
            else
            {
                objectAccess.EnabledFlg = accessFlag;
                Context.SaveChanges();
            }
            
            return Json(new { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteUserObjectAccess(int userId, int objectId, int communityId)
        {
            var user = Context.MasterBusinessUsers.Find(userId);
            var userAccess = user.SpecialAccess.FirstOrDefault(a => a.ObjectPermissionId == objectId && a.CommunityId == communityId);
            if (userAccess != null)
            {
                Context.ObjectUserPermission.Remove(userAccess);
                Context.SaveChanges();
            }

            return Json(new { Success = true });
        }

        [HttpPost]
        public JsonResult UpdateUserAppAccess(int userId,int appId, int communityId, bool reportFlag, bool appFlag)
        {
            var user = Context.MasterBusinessUsers.Find(userId);
            var userAccess = user.UserAccess.FirstOrDefault(a => a.ApplicationId == appId && a.CommunityId == communityId);
            if (userAccess == null)
            {
                userAccess = new ApplicationCommunityBusinessUserInfo()
                {
                    ApplicationId = appId,
                    CommunityId = communityId,
                    BusinessUserId = userId,
                    ReportFlg = reportFlag,
                    AppFlg = appFlag
                };
                Context.ApplicationUserAccess.Add(userAccess);
                Context.SaveChanges();
            }
            else
            {
                userAccess.ReportFlg = reportFlag;
                userAccess.AppFlg = appFlag;
                Context.SaveChanges();
            }

            return Json(new { Success = true }) ;
        }

        [HttpPost]
        public JsonResult DeleteUserAppAccess(int userId, int appId, int communityId)
        {
            var user = Context.MasterBusinessUsers.Find(userId);
            var userAccess = user.UserAccess.FirstOrDefault(a => a.ApplicationId == appId && a.CommunityId == communityId);
            if (userAccess != null)
            {
                Context.ApplicationUserAccess.Remove(userAccess);
                Context.SaveChanges();
            }

            return Json(new { Success = true });
        }

        public JsonResult GetCommunitiesForApp(int appId)
        {
            var communityIDList = Context.ApplicationCommunityInfos
                .Where(a => a.ApplicationId == appId && (a.DataEntryFlg || a.ReportFlg)).Select(a => a.CommunityId).ToList();
            return Json(new { CommunityIdList = communityIDList });
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

    }
}