using Microsoft.EntityFrameworkCore;
using AtlasPatient.Data.IRepository;
using AtlasPatient.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasPatient.Data.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _context;

        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<int?> IsExistingPatientAsync(string ssn)
        {
            return await _context.PatientDetail
                .Where(p => p.Ssn == ssn)
                .Select(p => (int?)p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<PatientDetail> GetPatientDetailAsync(int id)
        {
            return await _context.PatientDetail.FindAsync(id);
        }

        public async Task<int> AddPatientAsync(PatientDetail PatientDetail)
        {
            _context.PatientDetail.Add(PatientDetail);
            await _context.SaveChangesAsync();
            return PatientDetail.Id;
        }

        public async Task AddPatientLabVisitsAsync(IEnumerable<PatientLabVisit> labVisits)
        {
            _context.PatientLabVisits.AddRange(labVisits);
            await _context.SaveChangesAsync();
        }

        public async Task AddPatientMedicationsAsync(IEnumerable<PatientMedication> medications)
        {
            _context.PatientMedications.AddRange(medications);
            await _context.SaveChangesAsync();
        }

        public async Task AddPatientVaccinationsAsync(IEnumerable<PatientVaccinationDatum> vaccinations)
        {
            _context.PatientVaccinationDatum.AddRange(vaccinations);
            await _context.SaveChangesAsync();
        }
    }
}
