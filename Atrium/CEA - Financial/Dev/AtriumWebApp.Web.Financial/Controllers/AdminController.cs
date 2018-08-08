using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "POR,ADR,CON,WO", Admin = true)]
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

            return View(new AdminHomeViewModel
            {
                HomeViewModel = new HomeViewModel() { AccessByAppCode = AdminAccess },
                DatabaseName = databaseName,
                ServerName = serverName
            });
        }
    }
}