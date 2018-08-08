using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized",AppCode="SOC,ITR,HOD,IFC,VAC,EIFC,EVAC",Admin=true)]
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

        public ActionResult Index()
        {
            //Display Database Information
            var parser = new SqlConnectionStringBuilder(Context.Database.Connection.ConnectionString);
            var serverName = parser.DataSource;
            var databaseName = parser.InitialCatalog;


            if (AdminAccess == null)
            {
                AdminAccess = DetermineAdminAccess(PrincipalContext, UserPrincipal);
            }

            return View(new AdminHomeViewModel
            {
                HomeViewModel = new HomeViewModel() { AccessByAppCode = AdminAccess },
                DatabaseName = databaseName,
                ServerName = serverName
            });
        }

    }
}
