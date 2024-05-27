using AtlasPatient.Models.DTOs;
using AtlasPatient.Models.Models;
using AtlasPatient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AtlasPatient.Core;
using AtlasPatient.Data.IRepository;
using AtlasPatient.Core.IServices;
using Azure.Core;

namespace AtlasPatient.Core.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly HttpClient _httpClient;

        public PatientService(IPatientRepository patientRepository, HttpClient httpClient)
        {
            _patientRepository = patientRepository;
            _httpClient = httpClient;
        }

        public async Task<int?> IsExistingPatientAsync(string ssn)
        {
            return await _patientRepository.IsExistingPatientAsync(ssn);
        }

        public async Task<PatientDto> GetPatientDataAsync(int id)
        {
            var patient = await _patientRepository.GetPatientDetailAsync(id);
            if (patient == null) return null;

            return new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                LastName = patient.LastName,
                Dob = patient.Dob,
                Ssn = patient.Ssn,
                Address = patient.Address,
                City = patient.City,
                Zip = patient.Zip,
                State = patient.State
            };
        }

        public async Task<int> RegisterNewPatientAsync(PatientDto patientDto)
        {
            var patient = new PatientDetail
            {
                FirstName = patientDto.FirstName,
                MiddleName = patientDto.MiddleName,
                LastName = patientDto.LastName,
                Dob = patientDto.Dob,
                Ssn = patientDto.Ssn,
                Address = patientDto.Address,
                City = patientDto.City,
                Zip = patientDto.Zip,
                State = patientDto.State,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            //var patientId = 1;
            var patientId = await _patientRepository.AddPatientAsync(patient);

            return patientId;
        }

        public async Task SaveLabVisitsAsync(string ssn, int patientId, string token)
        {
            // Get lab visits
            var visitRequest = new HttpRequestMessage(HttpMethod.Get, $"https://testapi.mindware.us/patient-lab-visits?SSN={ssn}");
            visitRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var visitResponse = await _httpClient.SendAsync(visitRequest);
            visitResponse.EnsureSuccessStatusCode();
            var visits = await visitResponse.Content.ReadAsStringAsync();
            var labVisits = JsonSerializer.Deserialize<List<LabVisitDto>>(visits);

            var labVisitEntities = new List<PatientLabVisit>();
            var labResultEntities = new List<PatientLabResult>();

            foreach (var visit in labVisits)
            {
                var labVisitEntity = new PatientLabVisit
                {
                    PatientId = patientId,
                    LabName = visit.LabName,
                    LabTestRequest = visit.LabTestRequest,
                    ResultDate = visit.ResultDate.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                labVisitEntities.Add(labVisitEntity);

                // Fetching the Result Data
                var resultRequest = new HttpRequestMessage(HttpMethod.Get, $"https://testapi.mindware.us/Patient-lab-results?lab_visit_id={visit.id}");
                resultRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var resultResponse = await _httpClient.SendAsync(resultRequest);
                resultResponse.EnsureSuccessStatusCode();
                var labResults = JsonSerializer.Deserialize<List<LabResultDto>>(await resultResponse.Content.ReadAsStringAsync());

                foreach (var result in labResults)
                {
                    var labResultEntity = new PatientLabResult
                    {
                        LabVisitId = visit.id,
                        TestName = result.TestName,
                        TestResult = result.TestResult,
                        TestObservation = result.TestObservation,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    labResultEntities.Add(labResultEntity);
                }

            }

            // Save lab visit entities to the database
            await _patientRepository.AddPatientLabVisitsAsync(labVisitEntities);
            // Save lab result entities to the database
            await _patientRepository.AddPatientLabResultsAsync(labResultEntities);
        }


        public async Task SaveMedicationsAsync(string ssn, int patientId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://testapi.mindware.us/patient-vaccinations?SSN={ssn}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var medications = JsonSerializer.Deserialize<List<PatientMedication>>(await response.Content.ReadAsStringAsync());

            foreach (var med in medications)
            {
                med.PatientId = patientId;
                med.CreatedAt = DateTime.UtcNow;
                med.UpdatedAt = DateTime.UtcNow;
            }

            await _patientRepository.AddPatientMedicationsAsync(medications);
        }

        public async Task SaveVaccinationsAsync(string ssn, int patientId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://testapi.mindware.us/patient-medications?SSN={ssn}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var vaccinations = JsonSerializer.Deserialize<List<PatientVaccinationDatum>>(await response.Content.ReadAsStringAsync());

            foreach (var vacc in vaccinations)
            {
                vacc.PatientId = patientId;
                vacc.CreatedAt = DateTime.UtcNow;
                vacc.UpdatedAt = DateTime.UtcNow;
            }

            await _patientRepository.AddPatientVaccinationsAsync(vaccinations);
        }
    }
}
