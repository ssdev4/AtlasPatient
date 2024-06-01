using System;
namespace AtlasPatient.API.DataInjest
{
    public class LabVisitDataInjestEvent
    {
        public int PatientID { get; set; }
        public string SSN { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class MedicationDataInjestEvent
    {
        public int PatientID { get; set; }
        public string SSN { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class VaccinationDataInjestEvent
    {
        public int PatientID { get; set; }
        public string SSN { get; set; }
        public DateTime DateTime { get; set; }
    }
}

