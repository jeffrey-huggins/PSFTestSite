using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models.ViewModel
{
    public class ProviderListViewModel 
    {
        public bool CanManage { get; set; }
        
        public List<ProviderViewModel> Providers { get; set; }
        
        public ProviderViewModel CurrentProvider { get; set; }
    }
}
