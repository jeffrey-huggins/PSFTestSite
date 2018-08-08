using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace AtriumWebApp.Web.HR.Models
{
	public class EmployeeNewHire
	{
		[Key]
		public int EmployeeNewHireId { get; set; }
		[Required]
		public int EmployeeId { get; set; }
		[DataType(DataType.Date)]
		[Required]
		[DisplayName("Current Hire Date")]
		public DateTime CurrentHireDate { get; set; }
		[DisplayName("Completed")]
		public bool CompletedFlg { get; set; }

		[ForeignKey("EmployeeId")]
		public virtual Employee Employee { get; set; }

		public virtual List<EmployeeNewHireChecklist> CheckList { get; set; }

    }
}
