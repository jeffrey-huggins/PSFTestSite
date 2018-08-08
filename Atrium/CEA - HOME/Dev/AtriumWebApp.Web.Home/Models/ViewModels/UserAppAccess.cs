using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Home.Models.ViewModels
{
    public class UserAppAccess
    {
        public List<CommunityAccess> accessList { get; set; }
        public int userId { get; set; }
        public List<ObjectCommunityAccess> objAccessList { get; set; }
        public List<AdminAccess> adminAccessList { get; set; }
    }
}
