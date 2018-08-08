using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models.ViewModel
{
    public class ProviderActionResultViewModel : SaveResultViewModel
    {
        public ProviderViewModel Result { get; set; }
        public String ErrorMessage { get; set; }
    }
}