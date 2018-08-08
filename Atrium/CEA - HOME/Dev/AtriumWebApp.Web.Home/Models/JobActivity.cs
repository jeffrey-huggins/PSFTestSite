using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Home.Models
{
    public class JobActivity
    {
        public Guid Job_id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Last_run_date { get; set; }
        public int Last_run_time { get; set; }
        public int? Last_run_outcome { get; set; }
        public int? Current_execution_status { get; set; }
        public string Current_execution_step { get; set; }
        public DateTime? LastRunDate { get; set; }

        //public string Message { get; set; }
        //public int? Run_status { get; set; }
    }
}
