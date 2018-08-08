using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class TemplateSchdlSlotDay
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TemplateSchdlSlotId { get; set; }
        [Required]
        [Range(1,14)]
        public int PayPeriodDayNbr { get; set; }
        public int? AtriumPatientGroupId { get; set; }
        public DateTime? ShiftStartTime { get; set; }
        public DateTime? ShiftEndTime { get; set; }
        public decimal? HourCnt { get; set; }
        public int? SchdlHourAltId { get; set; }

        public virtual TemplateSchdlSlot Slot { get; set; }
        public virtual SystemSchdlHourAlt SchdlHourAlt { get; set; }
        public virtual MasterAtriumPatientGroup AtriumPatientGroup { get; set; }
    }
}