using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class WriteOffIndexViewModel
    {
        //public bool IsAdministrator { get; set; }
        public IList<WriteOffViewModel> Items { get; set; }
        [DisplayName("Community")]
        public int CurrentCommunity { get; set; }
        public IList<Community> Communities { get; set; }
        [DisplayName("From")]
        public DateTime DateRangeFrom { get; set; }
        [DisplayName("To")]
        public DateTime DateRangeTo { get; set; }
        //public WriteOff WriteOff { get; set; }
        public string AppCode { get; set; }
    }
}