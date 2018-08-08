using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.HR.Models
{
    public class EmployeeNewHireChecklist
    {
		[Key]
		public int EmployeeNewHireChecklistId { get; set; }
		[Required]
		public int EmployeeNewHireId { get; set; }
		[Required]
		public int NewHireChecklistId { get; set; }

		[ForeignKey("NewHireChecklistId")]
		public virtual MasterNewHireChecklist CheckListInfo { get; set; }

		[ForeignKey("EmployeeNewHireId")]
		public virtual EmployeeNewHire EmployeeNewHire { get; set; }

		public virtual List<EmployeeNewHireChecklistDocument> Documents { get; set; }
    }
}
