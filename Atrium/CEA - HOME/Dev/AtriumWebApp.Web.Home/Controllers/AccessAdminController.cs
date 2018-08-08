using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Home.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AtriumWebApp.Web.Home.Controllers
{
    [RestrictAccessWithApp(SysAdmin = true, RedirectUrl = "Unauthorized")]
    public class AccessAdminController : BaseAdminController
    {
        public AccessAdminController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            AdminUserAppListViewModel userAdminView = new AdminUserAppListViewModel()
            {
                UserList = Context.MasterBusinessUsers.Where(a => a.AccountName != null).ToList(),
                ApplicationList = Context.Applications.Where(a => a.ApplicationId > 0).OrderBy(a => a.ApplicationGroupId).ToList(),
                CommunityList = Context.Facilities.Where(a => a.CommunityId > 0).ToList(),
                ObjectAccessList = Context.ObjectPermission.Where(a => a.ApplicationId != 0).ToList()
            };

            return View(userAdminView);
        }

        [HttpPost]
        public JsonResult GetAppAccess(int appId)
        {
            var model = ModelState;
            List<AccessViewModel> accessList = new List<AccessViewModel>();
            var userAccess = Context.ApplicationUserAccess.Where(a => a.ApplicationId == appId && a.AppFlg).Select(a => a.User).Distinct().ToList();
            foreach (var access in userAccess)
            {
                if (accessList.Count(a => a.UserId == access.BusinessUserId) > 0)
                {
                    continue;
                }
                accessList.Add(new AccessViewModel()
                {
                    UserDisplayName = access.DisplayName,
                    UserId = access.BusinessUserId,
                    AccountName = access.AccountName
                });
                //access.User.DisplayName
                //access.User.BusinessUserId
            }

            //Context.ApplicationCommunityInfos
            //foreach(var appInfo)
            return new JsonResult(accessList);
        }

        [HttpPost]
        public JsonResult GetObjectAccess(int objectId)
        {
            List<AccessViewModel> accessList = new List<AccessViewModel>();
            SystemObjectPermission obj = Context.ObjectPermission.Find(objectId);
            foreach (var access in obj.UserPermissions)
            {
                if (accessList.Count(a => a.UserId == access.BusinessUserId) > 0)
                {
                    continue;
                }
                accessList.Add(new AccessViewModel()
                {
                    UserDisplayName = access.User.DisplayName,
                    UserId = access.BusinessUserId,
                    AccountName = access.User.AccountName
                });
            }

            return new JsonResult(accessList);
        }
        [HttpPost]
        public JsonResult GetCommunityInfo(int communityId)
        {
            var community = Context.Facilities.Find(communityId);
            return new JsonResult(community.AppCommInfo);
        }
        [HttpPost]
        public JsonResult SetCommunityInfo(int communityId, int applicationId, bool dataFlag, bool reportFlag)
        {
            var community = Context.Facilities.Find(communityId);
            var appCommInfo = community.AppCommInfo.FirstOrDefault(a => a.ApplicationId == applicationId);
            if(appCommInfo == null)
            {
                appCommInfo = new ApplicationCommunityInfo()
                {
                    CommunityId = communityId,
                    ApplicationId = applicationId,
                    DataEntryFlg = dataFlag,
                    ReportFlg = reportFlag
                };
                Context.ApplicationCommunityInfos.Add(appCommInfo);
                Context.SaveChanges();
                
            }
            else
            {
                appCommInfo.DataEntryFlg = dataFlag;
                appCommInfo.ReportFlg = reportFlag;
                Context.SaveChanges();
            }
            return new JsonResult(new { Success = true, data = appCommInfo });
        }

        [HttpPost]
        public JsonResult GetAdminAccess(int appId)
        {
            List<AccessViewModel> accessList = new List<AccessViewModel>();
            ApplicationInfo app = Context.Applications.Find(appId);
            foreach (var access in app.UserAdminList)
            {
                if (accessList.Count(a => a.UserId == access.BusinessUserId) > 0)
                {
                    continue;
                }
                accessList.Add(new AccessViewModel()
                {
                    UserDisplayName = access.User.DisplayName,
                    UserId = access.BusinessUserId,
                    AccountName = access.User.AccountName
                });
            }
            return new JsonResult(accessList);
        }

        [HttpPost]
        public JsonResult SetAppAccess(int appId, bool allow)
        {
            ApplicationInfo app = Context.Applications.Find(appId);
            app.EnabledFlg = allow;
            Context.SaveChanges();
            return new JsonResult(new { Success = true });

        }

        [HttpPost]
        public JsonResult SetObjectAccess(int objectId, bool allow)
        {
            SystemObjectPermission obj = Context.ObjectPermission.Find(objectId);
            obj.UserPermissions.ForEach(a => a.EnabledFlg = allow);
            Context.SaveChanges();
            return new JsonResult(new { Success = true });

        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }
    }
}