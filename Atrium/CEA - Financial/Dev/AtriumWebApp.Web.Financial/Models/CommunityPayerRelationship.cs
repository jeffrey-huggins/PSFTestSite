using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models
{
    public class CommunityPayerRelationship
    {
        [Key]
        [Column(Order=1)]
        public int CommunityId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int PayerId { get; set; }

        //public Community Community { get; set; }
        //public CommunityPayers CommunityPayer { get; set; }
    }
}