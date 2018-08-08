using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class Community
    {
        [Key]
        public int CommunityId { get; set; }
        public string AtriumCommunityCd { get; set; }
        public string CommunityShortName { get; set; }
        public string CommunityFullName { get; set; }
        public string StreetAddress { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string StateCd { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }
        public int RegionId { get; set; }
        public bool IsCommunityFlg { get; set; }
        [ForeignKey("StateCd")]
        public virtual State State { get; set; }

        [JsonIgnore]
        public virtual List<ApplicationCommunityInfo> AppCommInfo { get; set; }

        public string FullAddress
        {
            get
            {
                var str = new System.Text.StringBuilder();
                str.AppendLine(StreetAddress);
                if (!String.IsNullOrWhiteSpace(Suite))
                {
                    str.AppendLine(Suite);
                }
                str.Append(String.Format("{0}, {1} {2}", City, StateCd, !String.IsNullOrWhiteSpace(ZipCode) && ZipCode.Length > 5 ? ZipCode.Substring(0, 5) : ZipCode));

                return str.ToString();
            }
        }

        public string FullAddressRaw
        {
            get { return FullAddress.Replace("\r\n", "<br />"); }
        }
    }
    public class CommunityDTO
    {
        public int CommunityId { get; set; }
        public string CommunityFullName { get; set; }
        public string CommunityShortName { get; set; }
    }
    public class CommunityDTO2
    {
        public int CommunityId { get; set; }
        public string AtriumCommunityCd { get; set; }
        public string CommunityShortName { get; set; }
        public string CommunityFullName { get; set; }
        public string StateCd { get; set; }
        public int RegionId { get; set; }
        public bool IsCommunityFlg { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
