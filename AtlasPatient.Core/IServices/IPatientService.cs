using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlasPatient.Models.Models;
using AtlasPatient.Models.DTOs;

namespace AtlasPatient.Core.Services
{
    public interface IPatientService
    {
        Task<int?> IsExistingPatientAsync(string ssn);
        Task<PatientDto> GetPatientDataAsync(int id);
        Task<int> RegisterNewPatientAsync(PatientDto patientDto);
    }
}
