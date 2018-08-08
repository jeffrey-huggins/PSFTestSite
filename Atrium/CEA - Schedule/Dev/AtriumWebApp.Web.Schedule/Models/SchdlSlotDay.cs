using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class SchdlSlotDay
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int SchdlSlotId { get; set; }
        [Required]
        public DateTime WorkDate { get; set; }
        public int? AtriumPatientGroupId { get; set; }
        public DateTime? ShiftStartTime { get; set; }
        public DateTime? ShiftEndTime { get; set; }
        public decimal? HourCnt { get; set; }
        public int? SchdlHourAltId { get; set; }

        public virtual SchdlSlot Slot { get; set; }
        public virtual SystemSchdlHourAlt SchdlHourAlt { get; set; }
        public virtual MasterAtriumPatientGroup AtriumPatientGroup { get; set; }
    }
}