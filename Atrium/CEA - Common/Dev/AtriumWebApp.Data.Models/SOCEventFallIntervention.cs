using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCEventFallIntervention
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SOCEventId { get; set; }
        [Key]
        public int SOCFallInterventionId { get; set; }

        public SOCEventFall SOCEvent { get; set; }
    }
}
