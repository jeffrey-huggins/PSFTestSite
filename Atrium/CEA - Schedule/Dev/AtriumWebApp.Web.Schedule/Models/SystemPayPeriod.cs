using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class SystemPayPeriod
    {
        // PayPeriodId
        [Required]
        public int Id { get; set; }
        [Required]
        public int CommunityId { get; set; }
        public DateTime PayPeriodBeginDate { get; set; }
        public DateTime PayPeriodEndDate { get; set; }
        public DateTime CheckDate { get; set; }
        [StringLength(4)]
        public string PayPeriodYear { get; set; }
        [StringLength(2)]
        public string PayPeriod { get; set; }
        [StringLength(6)]
        public string PayPeriodTotal { get; set; }
    }
}