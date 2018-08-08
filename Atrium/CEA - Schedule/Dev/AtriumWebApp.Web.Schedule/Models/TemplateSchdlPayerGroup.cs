using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class TemplateSchdlPayerGroup
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TemplateSchdlPayPeriodId { get; set; }
        [Required]
        [StringLength(3)]
        public string AtriumPayerGroupCode { get; set; }
        public decimal AvgDailyCensusCnt { get; set; }

        public virtual TemplateSchdlPayPeriod TemplatePayPeriod { get; set; }
        public virtual AtriumPayerGroup PayerGroup { get; set; }
        public virtual ICollection<TemplateSchdlGeneralLedger> ScheduleLedger { get; set; }

    }
}