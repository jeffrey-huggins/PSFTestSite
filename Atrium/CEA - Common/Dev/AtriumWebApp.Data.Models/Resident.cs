using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        public string SrcSystemPatientId { get; set; }
        public string SrcSystemName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime AdmitDate { get; set; }
        public DateTime LastCensusDate { get; set; }
        public string SocialSecurityNumber { get; set; }
        public int CommunityId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string MedicareNumber { get; set; }
        public string MedicaidNumber { get; set; }
        public string MedicalRecordNumber { get; set; }
        public string LastStatus { get; set; }
        public int? CurrentPayerId { get; set; }
        public bool ShortStayFlg { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        [ForeignKey("CommunityId")]
        public virtual Community Community { get; set; }
    }

    public class PatientDTO
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoomId { get; set; }
        public int CommunityId { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime AdmitDate { get; set; }
        public DateTime LastCensusDate { get; set; }
    }
}
