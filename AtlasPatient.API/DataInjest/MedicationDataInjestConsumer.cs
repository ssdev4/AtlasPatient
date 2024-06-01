using System;
using System.Threading.Tasks;
using AtlasPatient.Core.IServices;
using AtlasPatient.Core.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AtlasPatient.API.DataInjest
{
    public class MedicationDataInjestConsumer : IConsumer<MedicationDataInjestEvent>
    {
        private readonly IPatientService _patientService;
        private readonly IAuthService _authService;
        private readonly ILogger<MedicationDataInjestConsumer> _logger;

        public MedicationDataInjestConsumer(IPatientService patientService, IAuthService authService, ILogger<MedicationDataInjestConsumer> logger)
        {
            _patientService = patientService;
            _authService = authService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MedicationDataInjestEvent> context)
        {
            var message = context.Message;
            var token = await _authService.GetAuthTokenAsync();

            try
            {
                await _patientService.SaveMedicationsAsync("111111111", message.PatientID, token);
                _logger.LogInformation($"Successfully processed medication data for PatientID: {message.PatientID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing medication data for PatientID: {message.PatientID}");
                throw;  // Rethrow the exception to trigger the retry policy
            }
        }
    }
}
