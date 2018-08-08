using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCEventFall : SOCEvent
    {
		[Required]
		[DisplayName("Time Occured")]
        public DateTime FallTime { get; set; }
		[Required]
		[DisplayName("Location")]
        public int SOCFallLocationId { get; set; }
        public bool PhysicianNotifiedFlg { get; set; }
        public bool FamilyNotifiedFlg { get; set; }
        public bool StateNotifiedFlg { get; set; }
        public string RootCause { get; set; }
    }
}
