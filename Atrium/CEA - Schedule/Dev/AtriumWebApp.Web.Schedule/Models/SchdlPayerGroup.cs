using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class SchdlPayerGroup
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int SchdlPayPeriodId { get; set; }
        [Required]
        [StringLength(3)]
        public string AtriumPayerGroupCode { get; set; }
        [Range(1.0, 5000.00, ErrorMessage = "The Census must be between 1 to 5000.00")]
        [DisplayFormat(DataFormatString = "{0:#.#}",ApplyFormatInEditMode=true)]
        public decimal AvgDailyCensusCnt { get; set; }

        public virtual SchdlPayPeriod PayPeriod { get; set; }
        public virtual AtriumPayerGroup PayerGroup { get; set; }
        public virtual IList<SchdlGeneralLedger> ScheduleLedger { get; set; }
    }
}
