using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Models
{
    public class ResidentGroup
    {
        public int Id { get; set; }
        [StringLength(25)]
        public string Name { get; set; }
        public int CommunityId { get; set; }
    }
}
