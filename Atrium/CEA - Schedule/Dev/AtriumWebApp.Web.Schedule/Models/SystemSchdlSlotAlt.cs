using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class SystemSchdlSlotAlt
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string SlotAltCode { get; set; }
        [Required]
        public string SlotAltDesc { get; set; }
        [Required]
        public int SortOrder { get; set; }
        [Required]
        public bool PPDCalcFlg { get; set; }
        [Required]
        public bool EmployeeCalcFlg { get; set; }
        [Required]
        public bool SchdlCalcFlg { get; set; }
    }
}