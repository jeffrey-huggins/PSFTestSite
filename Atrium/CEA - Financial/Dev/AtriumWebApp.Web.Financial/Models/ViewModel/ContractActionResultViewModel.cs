using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class ContractActionResultViewModel : SaveResultViewModel
    {
        public ContractViewModel Result { get; set; }
    }
}