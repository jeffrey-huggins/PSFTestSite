using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class MockSafetyCitation : BaseMockCitation
    {
        public SafetyDeficiency Deficiency
        {
            get { return (SafetyDeficiency)BaseDeficiency; }
            set { BaseDeficiency = value; }
        }
    }
}