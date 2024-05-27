using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasPatient.Models.DTOs
{
    public class LabVisitDto
    {
        public string LabName { get; set; }
        public string LabTestRequest { get; set; }
        public DateTime ResultDate { get; set; }
    }
}
