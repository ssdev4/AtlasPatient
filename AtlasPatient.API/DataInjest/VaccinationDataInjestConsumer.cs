using System;
using System.Threading.Tasks;
using AtlasPatient.Core.IServices;
using AtlasPatient.Core.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AtlasPatient.API.DataInjest
{
    public class VaccinationDataInjestConsumer : IConsumer<VaccinationDataInjestEvent>
    {
        private readonly IPatientService _patientService;
        private readonly IAuthService _authService;
        private readonly ILogger<VaccinationDataInjestConsumer> _logger;

        public VaccinationDataInjestConsumer(IPatientService patientService, IAuthService authService, ILogger<VaccinationDataInjestConsumer> logger)
        {
            _patientService = patientService;
            _authService = authService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VaccinationDataInjestEvent> context)
        {
            var message = context.Message;
            var token = await _authService.GetAuthTokenAsync();

            try
            {
                await _patientService.SaveVaccinationsAsync("111111111", message.PatientID, token);
                _logger.LogInformation($"Successfully processed vaccination data for PatientID: {message.PatientID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing vaccination data for PatientID: {message.PatientID}");
                throw;  // Rethrow the exception to trigger the retry policy
            }
        }
    }
}
