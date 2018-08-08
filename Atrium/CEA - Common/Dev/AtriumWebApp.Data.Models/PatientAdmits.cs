using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class PatientAdmits
    {
        [Key]
        public int PatientId { get; set; }

        [Key]
        public DateTime AdmitDateTime { get; set; }

        public int PayerId { get; set; }
        public string CensusType { get; set; }
        public string AdmitSrc { get; set; }
        public int VisitCount { get; set; }
    }
}