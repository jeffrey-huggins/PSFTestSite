using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class SystemScheduleLogItem
    {
        [Key]
        public int ScheduleLogId { get; set; }
        public int CommunityId { get; set; }
        public string ScheduleType { get; set; }
        public DateTime PayPeriodBeginDate { get; set; }
        public string Action { get; set; }
        public string ADName { get; set; }
    }
}