using System.Collections.Generic;

using AtriumWebApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class SurveyViewModel
    {
        public CommunitySurvey CurrentSurvey { get; set; }

        /// <summary>
        /// Documents for CurrentSurvey
        /// </summary>
        public ICollection<SurveyDocumentViewModel> Documents { get; set; }

        public IList<CivilMonetaryPenalty> CivilMonetaryPenalties { get; set; }

        public IList<FederalCitation> FederalCitations { get; set; }
        public IList<StateCitation> StateCitations { get; set; }
        public IList<SafetyCitation> SafetyCitations { get; set; }

        public IList<CommunitySurvey> Surveys { get; set; }

        public SelectList SurveyTypes { get; set; }

        public IList<CommunitySurveyType> SurveyTypesAll { get; set; }
        public SelectList SurveyPayerGroups { get; set; }

        public IList<FederalDeficiency> FederalDeficiencies { get; set; }
        public IList<StateDeficiency> StateDeficiencies { get; set; }
        public IList<SafetyDeficiency> SafetyDeficiencies { get; set; }

        public IList<ScopeAndSeverity> ScopeAndSeverity { get; set; }
        public SelectList Communities { get; set; }
        public int SelectedCommunity { get; set; }

        public decimal CMPYTD { get; set; }
        public int DaysOut { get; set; }

        public bool IsComplaint
        {
            get 
            {
                if (CurrentSurvey == null || SurveyTypes == null)
                    return false;
                var complaintType = SurveyTypes.Where(x => x.Value.Equals(CurrentSurvey.SurveyTypeId.ToString())).Select(x => x.Text).Single();
                return complaintType.Equals("Complaint");
            }
            
        }
    }
}