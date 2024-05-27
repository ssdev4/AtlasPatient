using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasPatient.Models.DTOs
{
    public class LabVisitDto
    {
        public int id { get; set; }
        public string SSN { get; set; }
        public string LabName { get; set; }
        public string LabTestRequest { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime ResultDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
