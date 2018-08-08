using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Home.Models.ViewModels
{
    public class ObjectCommunityAccess
    {
        public int ObjectPermissionId { get; set; }
        public bool EnabledFlag { get; set; }
        public bool RowExists { get; set; }
        public bool DeleteFlag { get; set; }
        public int? CommunityId { get; set; }
    }
}