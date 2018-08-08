using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class WriteOffViewModel
    {
        public int WriteOffId { get; set; }
        public int CommunityId { get; set; }
        public int PatientId { get; set; }
        public int PayerId { get; set; }
        public decimal WriteOffAmt { get; set; }
        public string DOSYear { get; set; }
        public string DOSMonth { get; set; }
        public bool OurFaultFlg { get; set; }
        public string Notes { get; set; }

        public IList<Community> Communities { get; set; }
        public IList<Patient> Patients { get; set; }

        public string CommunityName { get; set; }
        public string PatientName { get; set; }
        public string PayerName { get; set; }
    }
}