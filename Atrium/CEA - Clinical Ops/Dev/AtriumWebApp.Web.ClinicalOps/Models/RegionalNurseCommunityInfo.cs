using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class RegionalNurseCommunityInfo
    {
        [Key]
        public int RegionalNurseEmployeeId { get; set; }
        [Key]
        public int CommunityId { get; set; }
    }

    public class RegionalNurseComparer : IEqualityComparer<RegionalNurseCommunityInfo>
    {
        public bool Equals(RegionalNurseCommunityInfo x, RegionalNurseCommunityInfo y)
        {
            return x.RegionalNurseEmployeeId == y.RegionalNurseEmployeeId;
        }

        public int GetHashCode(RegionalNurseCommunityInfo regionalNurse)
        {
            //Check whether the object is null 
            if (ReferenceEquals(regionalNurse, null))
            {
                return 0;
            }
            //Get hash code for the Id field
            var hashregionalNurseName = regionalNurse.RegionalNurseEmployeeId.GetHashCode();
            //Calculate the hash code for the regionalNurse. 
            return hashregionalNurseName;
        }
    }
}