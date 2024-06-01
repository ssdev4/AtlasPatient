﻿using System.Threading.Tasks;
using AtlasPatient.Core.IServices;
using AtlasPatient.Core.Services;
using MassTransit;

namespace AtlasPatient.API.DataInjest
{
    public class VaccinationDataInjestConsumer : IConsumer<VaccinationDataInjestEvent>
    {
        private readonly IPatientService _patientService;
        private readonly IAuthService _authService;

        public VaccinationDataInjestConsumer(IPatientService patientService, IAuthService authService)
        {
            _patientService = patientService;
            _authService = authService;
        }

        public async Task Consume(ConsumeContext<VaccinationDataInjestEvent> context)
        {
            var message = context.Message;
            var token = await _authService.GetAuthTokenAsync();

            try
            {
                await _patientService.SaveVaccinationsAsync("111111111", message.PatientID, token);
            }
            catch (Exception ex)
            {
                // Log the error and handle retries if necessary
            }
        }
    }
}