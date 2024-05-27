using System;
namespace AtlasPatient.API.DataInjest
{
	public class DataInjestEvent
	{
		public int PatientID { get; set; }
        public string SSN { get; set; }
        public DateTime DateTime { get; set; }
    }
}

