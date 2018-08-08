using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;


namespace AtriumWebApp.Web.Schedule.Models
{
    public class TemplateSchdlGeneralLedger
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TemplateSchdlPayerGroupId { get; set; }
        [Required]
        public int GeneralLedgerId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n0000}")]
        public decimal HourPPDCnt { get; set; }

        public virtual TemplateSchdlPayerGroup TemplatePayerGroup { get; set; }
        public virtual GeneralLedgerAccount GeneralLedger { get; set; }
        public virtual ICollection<TemplateSchdlSlot> Slots { get; set; }
    }
}