using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.PayrollTransfer.Models.ViewModel
{
    public class PayrollTransferAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public IList<PTContractor> Contractors { get; set; }
        public IList<Community> Communities { get; set; }
        public PTContractor NewContract { get { return new PTContractor(); } }
    }
}