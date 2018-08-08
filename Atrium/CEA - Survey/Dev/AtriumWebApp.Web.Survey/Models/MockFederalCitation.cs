using System;

namespace AtriumWebApp.Web.Survey.Models
{
    public class MockFederalCitation : BaseMockCitation
    {
        public FederalDeficiency Deficiency
        {
            get { return (FederalDeficiency)BaseDeficiency; }
            set { BaseDeficiency = value; }
        }
    }
}