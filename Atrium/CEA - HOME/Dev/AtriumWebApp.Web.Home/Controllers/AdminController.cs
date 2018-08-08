using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Home.Models;
using AtriumWebApp.Web.Home.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Home.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        private IDictionary<string, bool> AdminAccess
        {
            get
            {
                Dictionary<string, bool> admin = new Dictionary<string, bool>();
                if (Session.TryGetObject("adminGroups", out admin))
                {
                    return admin;
                }

                return null;
            }
            set { Session.SetItem("adminGroups", value); }
        }

        public IActionResult Index()
        {
            //Display Database Information
            var parser = new SqlConnectionStringBuilder(Context.Database.Connection.ConnectionString);
            var serverName = parser.DataSource;
            var databaseName = parser.InitialCatalog;


            if (AdminAccess == null)
            {
                AdminAccess = DetermineAdminAccess(PrincipalContext, UserPrincipal);
            }

            //List<SSISItems> ssisItems = new List<SSISItems>();
            //string directoryPath = @"\\IRONMAN\dev\Job\SSIS\";
            //foreach (string directory in Directory.EnumerateDirectories(directoryPath))
            //{
            //    foreach(string fileName in Directory.EnumerateFiles(directory, "*.dtsx"))
            //    {
            //        ssisItems.Add(new SSISItems()
            //        {
            //            Category = directory,
            //            Name = fileName
            //        });
            //    }
            //}
            //ViewBag.ssisItems = ssisItems;
            return View(new AdminHomeViewModel
            {
                HomeViewModel = new HomeViewModel() { AccessByAppCode = AdminAccess },
                DatabaseName = databaseName,
                ServerName = serverName
            });
        }

        public IActionResult GetPackageStatus()
        {
            //SSISJobsContext jobContext = new SSISJobsContext(@"Data Source=IRONMAN,22866;Initial Catalog=msdb;Integrated Security=SSPI;");
            //Database.SetInitializer<SSISJobsContext>(null);
            var status = Context.Database.SqlQuery<JobActivity>("msdb.dbo.sp_help_job").Where(a => a.Category == "Business User SSIS Job").ToList();
            for(var i = 0; i < status.Count; i++)
            {
                int year = status[i].Last_run_date / 10000;
                int month = (status[i].Last_run_date / 100) % 100;
                int day = status[i].Last_run_date % 100;
                int hour = status[i].Last_run_time / 10000;
                int minute = (status[i].Last_run_time / 100) % 100;
                status[i].LastRunDate = new DateTime(year, month, day, hour, minute, 00);
            }
            return PartialView(status);
        }

        public IActionResult ExecutePackage(string package)
        {
            //SSISJobsContext jobContext = new SSISJobsContext(@"Data Source=IRONMAN,22866;Initial Catalog=msdb;Integrated Security=SSPI;");
            //Database.SetInitializer<SSISJobsContext>(null);
            if (Context.Database.SqlQuery<JobActivity>("msdb.dbo.sp_help_job").Where(a => a.Name == package && a.Current_execution_status == 1).Any())
            {
                return Json(new { status = "Job is already in progress." });
            }
            var status = Context.Database.ExecuteSqlCommand("msdb.dbo.sp_start_job @job_name", new SqlParameter("job_name", package));
            return Json(new { status });
        }

        public IActionResult Status()
        {
            var apps = Context.Applications.ToList();
            ServerStatusViewModel status = new ServerStatusViewModel()
            {
                Apps = apps
            };
            return View(status);
        }

    }
}