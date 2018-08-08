using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Home.Models.ViewModels
{
    public class AdminUserViewModel
    {
        public MasterBusinessUser User { get; set; }
        public List<ApplicationCommunityBusinessUserInfo> CurrentAccess { get; set; }
        public List<SystemAppAdmin> CurrentAppAccess { get; set; }
    }
}