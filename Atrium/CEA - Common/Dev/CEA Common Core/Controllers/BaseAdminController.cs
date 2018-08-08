using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Base.Controllers
{
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public static void SaveLookbackToApp(HttpContext context, string lookbackDays, string appCode)
        {
            using (var appContext = new SharedContext())
            {
                var appInfo = (from app in appContext.Applications
                               where app.ApplicationCode == appCode
                               select app).Single();
                var lbDays = Int32.Parse(lookbackDays);
                appInfo.LookbackDays = lbDays;
                appContext.SaveChanges();
                context.Session.SetItem(appCode + "LookbackDays", lbDays);
            }
        }

        protected AdminViewModel CreateAdminViewModel(string appCode)
        {
            using (var sharedContext = new SharedContext())
            {
                var appId = sharedContext.Applications.Single(x => x.ApplicationCode == appCode).ApplicationId;

                var comList = from com in sharedContext.Facilities
                              join app in sharedContext.ApplicationCommunityInfos
                                       on com.CommunityId equals app.CommunityId
                              where app.ApplicationId == appId
                              orderby com.IsCommunityFlg descending, com.CommunityShortName
                              select com;
                return new AdminViewModel
                {
                    AppId = appId,
                    ApplicationCommunityInfos = sharedContext.ApplicationCommunityInfos.Where(apc => apc.ApplicationId == appId).ToList(),
                    Communities = comList.ToList(),
                    Regions = sharedContext.MasterRegion.ToList()
                };
            }
        }
        public JsonResult GetDatabaseInfo()
        {
            //Display Database Information
            var parser = new SqlConnectionStringBuilder(Context.Database.Connection.ConnectionString);
            var serverName = parser.DataSource;
            var databaseName = parser.InitialCatalog;

            return Json(new { serverName = serverName, databaseName = databaseName });

        }



    }
}