using System.Collections.Generic;

using AtriumWebApp.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class SurveyAdminViewModel
    {
        public IList<CommunitySurveyType> SurveyTypes { get; set; }
        public IList<FederalDeficiency> FederalDeficiencies { get; set; }
        public IList<StateDeficiency> StateDeficiencies { get; set; }
        public IList<SafetyDeficiency> SafetyDeficiencies { get; set; }
        public IList<ScopeAndSeverity> SASs { get; set; }

        public SelectList SurveyPayerGroups { get; set; }
        public SelectList StateCodes { get; set; }
        public AdminViewModel AdminViewModel { get; set; }
        public ScopeAndSeverity SASModel
        {
            get
            {
                return new ScopeAndSeverity();
            }
            set {
                SASModel = value;
            }
        }
        public FederalDeficiencyViewModel FedDef
        {
            get
            {
                return new FederalDeficiencyViewModel()
                {
                    Deficiency = new FederalDeficiency(),
                    SurveyPayerGroups = SurveyPayerGroups
                };
            }
            set
            {
                FedDef = value;
            }
        }
        public StateDeficiencyViewModel StateDef
        {
            get
            {
                return new StateDeficiencyViewModel()
                {
                    Deficiency = new StateDeficiency(),
                    StateCodes = StateCodes
                };
            }
            set
            {
                StateDef = value;
            }
        }
        public SafetyDeficiency SafetyDef
        {
            get
            {
                return new SafetyDeficiency();
            }
            set
            {
                SafetyDef = value;
            }
        }
        public CommunitySurveyType SurveyType
        {
            get
            {
                return new CommunitySurveyType();
            }
            set
            {
                SurveyType = value;
            }
        }
        /// <summary>
        /// Documents for CurrentSurvey
        /// </summary>
        public ICollection<SurveyDocumentViewModel> Documents { get; set; }

    }
}