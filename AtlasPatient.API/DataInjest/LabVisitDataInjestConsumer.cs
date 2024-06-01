using System;
using System.Threading.Tasks;
using AtlasPatient.Core.IServices;
using AtlasPatient.Core.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AtlasPatient.API.DataInjest
{
    public class LabVisitDataInjestConsumer : IConsumer<LabVisitDataInjestEvent>
    {
        private readonly IPatientService _patientService;
        private readonly IAuthService _authService;
        private readonly ILogger<LabVisitDataInjestConsumer> _logger;

        public LabVisitDataInjestConsumer(IPatientService patientService, IAuthService authService, ILogger<LabVisitDataInjestConsumer> logger)
        {
            _patientService = patientService;
            _authService = authService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<LabVisitDataInjestEvent> context)
        {
            var message = context.Message;
            var token = await _authService.GetAuthTokenAsync();

            try
            {
                await _patientService.SaveLabVisitsAsync("111111111", message.PatientID, token);
                _logger.LogInformation($"Successfully processed lab visit data for PatientID: {message.PatientID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing lab visit data for PatientID: {message.PatientID}");
                throw;  // Rethrow the exception to trigger the retry policy
            }
        }
    }
}
