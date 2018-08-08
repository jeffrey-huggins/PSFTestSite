using System;
using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Home.Models.ViewModels
{
    public class AdminAppInfoViewModel
    {
        public ApplicationInfo AppInfo { get; set; }
        public List<MasterApplicationGroup> Groups {get;set;}
        public IEnumerable<SelectListItem> AppGroup
        {
            get { return new SelectList(Groups, "ApplicationGroupId", "ApplicationGroupName"); }
        }

    }
}