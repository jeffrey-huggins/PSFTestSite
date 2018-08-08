using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.HR.Models
{
    public class STNATrainingClass
    {
        [Key]
        public int STNATrainingClassesId { get; set; }
        public int STNATrainingFacilityId { get; set; }
        public string TrainerName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ClassType { get; set; }
        public int StudentsInClassCnt { get; set; }
        public int StudentsHiredCnt { get; set; }

    }
}