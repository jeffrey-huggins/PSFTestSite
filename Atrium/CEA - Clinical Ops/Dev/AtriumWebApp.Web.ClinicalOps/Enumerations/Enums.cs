using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Enumerations
{
    public enum IFCCode
    {
        Diagnosis = 0,
        Site = 1,
        Symptom = 2,
        Precaution = 3,
        Vaccine = 4,
        Organism = 5,
        Antibiotic = 6
    }

    public enum SOCCode
    {
        Pressure = 0,
        Composite = 1,
        FallLocation = 2,
        FallInjuryType = 3,
        FallTreatment = 4,
        FallIntervention = 5,
        FallType = 6,
        AntiPsychotic = 7,
        Catheter = 8,
        Restraint = 9,
        Measure = 10,
        AntiPsychoticMed = 11
    }

    public enum SOCWoundFlag
    {
        Threshold = 0,
        Length = 1,
        Width = 2,
        Depth = 3
    }

    public enum RMWCode
    {
        Insurance = 0,
        LawFirm = 1,
        VOC = 2,
        TCM = 3,
        Type = 4
    }

    public enum ITRCode
    {
        Incident = 0,
        Location = 1,
        Intervention = 2,
        Treatment = 3
    }

    public enum HODCode
    {
        ERDischarge = 0,
        HospitalDischarge = 1,
        DNRR = 2,
        Hospital = 3
    }

    public enum QRVWCode
    {
        SOCMeasure = 0,
        SOCQuestion = 1,
        OCS = 2,
        OCSQuestion = 3,
        OQ = 4
    }
}
