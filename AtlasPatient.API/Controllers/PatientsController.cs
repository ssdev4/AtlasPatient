﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AtlasPatient.Core.Services;
using AtlasPatient.Models.DTOs;
using MassTransit;
using AtlasPatient.API.DataInjest;

namespace AtlasPatient.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IPublishEndpoint _publishEndpoint;

        public PatientsController(IPatientService patientService, IPublishEndpoint publishEndpoint)
        {
            _patientService = patientService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("/exists")]
        public async Task<ActionResult<int?>> IsExistingPatient([FromQuery] string ssn)
        {
            var result = await _patientService.IsExistingPatientAsync(ssn);
            if (result == null)
                return NotFound("Patient not found.");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientData(int id)
        {
            var patient = await _patientService.GetPatientDataAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<int>> RegisterNewPatient([FromBody] PatientDto patientDto)
        {
            try
            {
                var patientId = await _patientService.RegisterNewPatientAsync(patientDto);

                if(patientId != null)
                {
                    await _publishEndpoint.Publish(new DataInjestEvent
                    {
                        PatientID = patientId,
                        SSN = patientDto.Ssn,
                        DateTime = DateTime.Now
                    });
                }
                return CreatedAtAction(nameof(GetPatientData), new { id = patientId }, patientId);
            }
            catch (Exception ex)
            {
                return Ok();
            }
            
        }
    }
}