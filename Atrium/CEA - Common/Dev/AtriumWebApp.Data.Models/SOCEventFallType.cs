using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCEventFallType
    {
        [Key]
        public int SOCEventId { get; set; }
        [Key]
        public int SOCFallTypeId { get; set; }

        public SOCEventFall SOCEvent { get; set; }
    }
}
