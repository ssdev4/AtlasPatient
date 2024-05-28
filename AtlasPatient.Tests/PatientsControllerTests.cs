using AtlasPatient.API.Controllers;
using AtlasPatient.Core.IServices;
using AtlasPatient.Core.Services;
using AtlasPatient.Models.DTOs;
using AtlasPatient.API.DataInjest;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Collections.Generic;

namespace AtlasPatient.Tests
{
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _mockPatientService = new Mock<IPatientService>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _controller = new PatientsController(_mockPatientService.Object, _mockPublishEndpoint.Object);
        }

        [Fact]
        public async Task IsExistingPatient_ReturnsOkResult_WithPatientId()
        {
            // Arrange
            var ssn = "123-45-6789";
            var patientId = 1;
            _mockPatientService.Setup(s => s.IsExistingPatientAsync(ssn)).ReturnsAsync(patientId);

            // Act
            var result = await _controller.IsExistingPatient(ssn);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(patientId, okResult.Value);
        }

        [Fact]
        public async Task IsExistingPatient_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            var ssn = "123-45-6789";
            _mockPatientService.Setup(s => s.IsExistingPatientAsync(ssn)).ReturnsAsync((int?)null);

            // Act
            var result = await _controller.IsExistingPatient(ssn);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPatientData_ReturnsOkResult_WithPatientDto()
        {
            // Arrange
            var patientId = 1;
            var patientDto = new PatientDto { Id = patientId, FirstName = "John", LastName = "Doe" };
            _mockPatientService.Setup(s => s.GetPatientDataAsync(patientId)).ReturnsAsync(patientDto);

            // Act
            var result = await _controller.GetPatientData(patientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(patientDto, okResult.Value);
        }

        [Fact]
        public async Task GetPatientData_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            var patientId = 1;
            _mockPatientService.Setup(s => s.GetPatientDataAsync(patientId)).ReturnsAsync((PatientDto)null);

            // Act
            var result = await _controller.GetPatientData(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task RegisterNewPatient_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var patientDto = new PatientDto { Ssn = "123-45-6789", FirstName = "John", LastName = "Doe" };
            var patientId = 1;
            _mockPatientService.Setup(s => s.RegisterNewPatientAsync(patientDto)).ReturnsAsync(patientId);

            // Act
            var result = await _controller.RegisterNewPatient(patientDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetPatientData), createdAtActionResult.ActionName);
            Assert.Equal(patientId, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(patientId, createdAtActionResult.Value);
        }

        [Fact]
        public async Task RegisterNewPatient_PublishesDataInjestEvent()
        {
            // Arrange
            var patientDto = new PatientDto { Ssn = "123-45-6789", FirstName = "John", LastName = "Doe" };
            var patientId = 1;
            _mockPatientService.Setup(s => s.RegisterNewPatientAsync(patientDto)).ReturnsAsync(patientId);

            // Act
            await _controller.RegisterNewPatient(patientDto);

            // Assert
            _mockPublishEndpoint.Verify(x => x.Publish(It.Is<DataInjestEvent>(e =>
                e.PatientID == patientId && e.SSN == patientDto.Ssn), default), Times.Once);
        }
    }
}
