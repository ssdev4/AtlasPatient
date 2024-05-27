using AtlasPatient.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasPatient.Data.IRepository
{
    public interface IPatientRepository
    {
        Task<int?> IsExistingPatientAsync(string ssn);
        Task<PatientDetail> GetPatientDetailAsync(int id);
        Task<int> AddPatientAsync(PatientDetail PatientDetail);
        Task AddPatientLabVisitsAsync(IEnumerable<PatientLabVisit> labVisits);
        Task AddPatientLabResultsAsync(IEnumerable<PatientLabResult> labResults);
        Task AddPatientMedicationsAsync(IEnumerable<PatientMedication> medications);
        Task AddPatientVaccinationsAsync(IEnumerable<PatientVaccinationDatum> vaccinations);
    }
}
