using System;
using AtlasPatient.Core.IServices;
using AtlasPatient.Core.Services;
using MassTransit;

namespace AtlasPatient.API.DataInjest
{
	public class DataInjestConsumer : IConsumer<DataInjestEvent>
    {
        private readonly IPatientService _patientService;
        private readonly IAuthService _authService;


        public DataInjestConsumer(IPatientService patientService, IAuthService authService)
		{
            _patientService = patientService;
            _authService = authService;
        }


        public async Task Consume(ConsumeContext<DataInjestEvent> context)
        {
            var message = context.Message;

            Console.WriteLine(message.SSN + " - " + message.DateTime.ToString());

            var token = await _authService.GetAuthTokenAsync();

            // SSN is fixed here
            await _patientService.SaveLabVisitsAsync("111111111", message.PatientID, token);
            await _patientService.SaveMedicationsAsync("111111111", message.PatientID, token);
            await _patientService.SaveVaccinationsAsync("111111111", message.PatientID, token);

            Console.WriteLine("Date Injested Successfully...............");
        }
    }
}
