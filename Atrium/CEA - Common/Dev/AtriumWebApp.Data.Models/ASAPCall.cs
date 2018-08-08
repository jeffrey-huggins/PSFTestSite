using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class ASAPCall
    {
        [Key]
        public int ASAPCallId { get; set; }
        public DateTime CallDate { get; set; }
        public int CommunityId { get; set; }
        public int ASAPComplaintTypeId { get; set; }
        public string CallerName { get; set; }
        public string CallerContactInfo { get; set; }
        public string ComplaintDesc { get; set; }
        public string InvestigationDesc { get; set; }
        public string ActionDesc { get; set; }
        public string SummaryDesc { get; set; }
    }
}
