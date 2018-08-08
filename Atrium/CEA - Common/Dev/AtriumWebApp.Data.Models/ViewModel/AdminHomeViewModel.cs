using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.ViewModel
{
    public class AdminHomeViewModel
    {
        public HomeViewModel HomeViewModel { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
    }
}
