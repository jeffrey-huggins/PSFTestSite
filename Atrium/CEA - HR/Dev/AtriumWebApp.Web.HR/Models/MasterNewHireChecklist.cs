using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.HR.Models
{
	public class MasterNewHireChecklist
	{
		[Key]
		public int NewHireChecklistId { get; set; }
		[Required]
		[MaxLength(512)]
		[DisplayName("Name")]
		public string CheckListName { get; set; }
		[Required]
		[DisplayName("Sort Order")]
		public int SortOrder { get; set; }
		[Required]
		[DataType(DataType.Date)]
		[DisplayName("Begin Date")]
		public DateTime EffectiveBeginDate { get; set; }
		[DataType(DataType.Date)]
		[DisplayName("End Date")]
		public DateTime EffectiveEndDate { get; set; }

		public virtual List<EmployeeNewHireChecklist> CheckLists { get; set; }


	}
}
