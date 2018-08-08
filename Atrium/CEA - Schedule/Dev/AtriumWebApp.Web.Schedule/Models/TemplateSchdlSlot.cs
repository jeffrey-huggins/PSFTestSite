using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class TemplateSchdlSlot
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TemplateSchdlGeneralLedgerId { get; set; }
        [Required]
        public int WorkShiftId { get; set; }
        [Required]
        public int SlotNbr { get; set; }
        public int? EmployeeId { get; set; }
        public int? SchdlSlotAltId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual TemplateSchdlGeneralLedger Ledger { get; set; }
        public virtual IList<TemplateSchdlSlotDay> Days { get; set; }
        public virtual SystemSchdlSlotAlt SchdlSlotAlt { get; set; }
    }
}