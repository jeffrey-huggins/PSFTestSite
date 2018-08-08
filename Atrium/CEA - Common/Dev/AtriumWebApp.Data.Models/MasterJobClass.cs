using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class MasterJobClass
    {
        [Key]
        public int JobClassId { get; set; }
        [MaxLength(10)]
        public string SrcSystemCompanyId { get; set; }
        [MaxLength(20)]
        public string SrcSystemName { get; set; }
        [MaxLength(20)]
        public string SrcSystemJobClassId { get; set; }
        [MaxLength(100)]
        public string JobDescription { get; set; }

        public virtual List<EmployeeJobClass> EmployeeJobClasses { get; set; }
    }
}
