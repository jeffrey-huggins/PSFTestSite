using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models
{
    public class WriteOff
    {
        [Key]
        public int WriteOffId { get; set; }
        public int CommunityId {get;set;}
        public int PatientId {get;set;}
        public int PayerId {get;set;}
        public decimal WriteOffAmt {get;set;}
        public string DOSYear {get;set;}
        public string DOSMonth {get;set;}
        public bool OurFaultFlg {get;set;}
        public string Notes {get;set;}
    }
}