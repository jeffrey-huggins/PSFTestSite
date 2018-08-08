using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class ApplicationCommunityInfo
    {
        [Key, Column(Order = 0)]
        public int ApplicationId { get; set; }
        [Key, Column(Order = 1)]
        public int CommunityId { get; set; }
        public bool DataEntryFlg { get; set; }
        public bool ReportFlg { get; set; }

        [ForeignKey("CommunityId")]
        [JsonIgnore]
        public virtual Community Community { get; set; }
        [ForeignKey("ApplicationId")]
        [JsonIgnore]
        public virtual ApplicationInfo Application { get; set; }
    }
}