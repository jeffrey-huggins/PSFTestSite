using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCEventCatheter : SOCEvent
    {
        public int SOCCatheterTypeId { get; set; }
		[ForeignKey("SOCCatheterTypeId")]
		public virtual SOCCatheterType CatheterType { get; set; }
    }
}
