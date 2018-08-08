using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.ViewModel
{
    public class ResidentViewModel
    {
		public string ResidentName { get; set; }
		public string CommunityName { get; set; }
		public string RoomName { get; set; }
		public string BirthDate { get; set; }
		public string AdmitDate { get; set; }
		public string LastCensusDate { get; set; }
		public string AdmissionDiagnosis { get; set; }
		public List<string>OtherDiagnosis { get; set; }

	}
}
