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
    public class ApplicationCommunityAtriumPayerGroupInfo
    {
        [Key, Column(Order = 0)]
        public int ApplicationId { get; set; }
        [Key, Column(Order = 1)]
        public int CommunityId { get; set; }
        [Key, Column(Order = 2)]
        public string AtriumPayerGroupCode { get; set; }
        public bool IncludeFlg { get; set; }
    }
}
