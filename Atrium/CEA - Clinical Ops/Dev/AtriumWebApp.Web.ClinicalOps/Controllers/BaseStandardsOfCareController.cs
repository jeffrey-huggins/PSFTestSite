using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Base.Controllers
{
    public class BaseStandardsOfCareController : BaseController
    {
        protected const string AppCode = "SOC";

        public BaseStandardsOfCareController(IOptions<AppSettingsConfig> config, SOCContext context) : base(config, context)
        {
        }

        protected new SOCContext Context
        {
            get { return (SOCContext)base.Context; }
        }
    }
}