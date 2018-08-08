using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.HR.Models
{
    public class EmployeeNewHireChecklistDocument
    {
		[Key]
		public int EmployeeNewHireChecklistDocumentId { get; set; }
		[Required]
		public int EmployeeNewHireChecklistId { get; set; }
		[Required]
		[MaxLength(256)]
		public string DocumentFileName { get; set; }
		[MaxLength(256)]
		public string ContentType { get; set; }
		[Required]
		public byte[] Document { get; set; }

		[ForeignKey("EmployeeNewHireChecklistId")]
		public virtual EmployeeNewHireChecklist ChecklistInfo { get; set; }
    }
}
