using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class CommunityPayers
    {
        [Key]
        public int PayerId { get; set; }
        public string AtriumPayerTypeCode { get; set; }
        public string PayerTypeName { get; set; }
        public string PayerName { get; set; }
    }
}
