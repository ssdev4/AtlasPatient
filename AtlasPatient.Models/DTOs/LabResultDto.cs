using System;
namespace AtlasPatient.Models.DTOs
{
	public class LabResultDto
	{
        public int LabVisitId { get; set; }
        public string TestName { get; set; }
        public string TestResult { get; set; }
        public string TestObservation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Attachments { get; set; }
    }
}

