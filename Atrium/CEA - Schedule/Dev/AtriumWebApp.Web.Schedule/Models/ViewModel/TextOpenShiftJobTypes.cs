using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class TextOpenShiftJobTypes
    {
        public bool IsAdministrator { get; set; }
        public int JobTypeId { get; set; }
        public string JobTypeName { get; set; }
        public bool Selected { get; set; }
    }
}