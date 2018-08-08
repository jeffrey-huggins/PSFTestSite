using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Home.Models.ViewModels
{
    public class AdminUserAppListViewModel
    {
        public List<ApplicationInfo> ApplicationList { get; set; }
        public List<MasterBusinessUser> UserList { get; set; }
        public List<Community> CommunityList { get; set; }
        public List<SystemObjectPermission> ObjectAccessList { get; set; }
        public int? SelectedUser { get; set; }
    }
}