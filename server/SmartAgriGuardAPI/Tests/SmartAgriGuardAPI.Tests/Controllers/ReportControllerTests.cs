using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Controllers;
using ApplicationLayer.Interfaces;
using ApplicationLayer.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using InfrastructureLayer.Services;

namespace SmartAgriGuardAPI.Tests.Controllers
{
    public class ReportControllerTests
    {
        private readonly Mock<IReportServcie> _mockReportService;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _mockReportService = new Mock<IReportServcie>();
            _controller = new ReportController(_mockReportService.Object);
        }

        [Fact]
        public async Task GenerateReport_ReturnsBadRequest_WhenFormatIsInvalid()
        {
             // Arrange
            var dto = new ReportRequestDTO { ReportFormat = "invalid" };

            // Act
            var result = await _controller.GenerateReport(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unsupported report format. Please use 'pdf' or 'excel'.", badRequestResult.Value);
        }

        // Note: Testing successful generation is difficult due to hard dependencies on ReportGeneratorContext and Strategies inside the controller.
        // We can at least test that validation works.
    }
}
