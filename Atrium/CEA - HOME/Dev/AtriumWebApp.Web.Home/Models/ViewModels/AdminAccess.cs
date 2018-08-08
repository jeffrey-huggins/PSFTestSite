using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Home.Models.ViewModels
{
    public class AdminAccess
    {
        public bool IsAdmin { get; set; }
        public bool DeleteRow { get; set; }
        public int AppId { get; set; }
    }
}