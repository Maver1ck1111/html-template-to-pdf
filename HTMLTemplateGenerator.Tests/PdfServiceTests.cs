using FluentAssertions;
using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
using HTMLTemplateGenerator.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemplateGenerator.Tests
{
    public class PdfServiceTests
    {
        private readonly Mock<ILogger<PdfService>> _mockLogger = new Mock<ILogger<PdfService>>();
        private readonly Mock<IHTMLRepository> _mockHtmlRepository = new Mock<IHTMLRepository>();

        [Fact]
        public async Task ConvertHtmlToPdfAsync_ShouldReturnCorrectResponse()
        {
            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            _mockHtmlRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Success(template));

            var service = new PdfService(_mockLogger.Object, _mockHtmlRepository.Object);

            var result = await service.ConvertHtmlToPdfAsync(template.Id);

            result.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();

            var (pdfBytes, fileName) = result.Value;

            pdfBytes.Length.Should().BeGreaterThan(0);

            pdfBytes.Take(4).SequenceEqual(Encoding.ASCII.GetBytes("%PDF")).Should().BeTrue();
            fileName.Should().Be(template.Name);
        }

        [Fact]
        public async Task ConvertHtmlToPdfAsync_ShouldReturnNotFound_WhenTemplateDoesNotExist()
        {
            _mockHtmlRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Failure(404, "Template not found"));

            var service = new PdfService(_mockLogger.Object, _mockHtmlRepository.Object);

            var result = await service.ConvertHtmlToPdfAsync(Guid.NewGuid());

            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ConvertHtmlToPdfAsync_ShouldReturnServerError_WhenRepositoryFails()
        {
            _mockHtmlRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Failure(500, "Database error"));

            var service = new PdfService(_mockLogger.Object, _mockHtmlRepository.Object);

            var result = await service.ConvertHtmlToPdfAsync(Guid.NewGuid());

            result.StatusCode.Should().Be(500);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ConvertHtmlToPdfAsync_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var service = new PdfService(_mockLogger.Object, _mockHtmlRepository.Object);

            var result = await service.ConvertHtmlToPdfAsync(Guid.Empty);

            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }
    }
}
