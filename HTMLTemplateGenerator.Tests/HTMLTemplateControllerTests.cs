using FluentAssertions;
using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
using HTMLTemplateGenerator.Application.ServicesContracts;
using HTMLTemplateGenerator.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HTMLTemplateGenerator.Tests
{
    public class HTMLTemplateControllerTests
    {
        private readonly Mock<ILogger<HTMLTemplateController>> _mockLogger = new Mock<ILogger<HTMLTemplateController>>();
        private readonly Mock<IHTMLRepository> _mockRepository = new Mock<IHTMLRepository>();
        private readonly Mock<IPdfService> _mockPdfService = new Mock<IPdfService>();

        #region CreateHTMLTemplate
        [Fact]
        public async Task CreateHTMLTemplate_ShouldReturnCorrectResponse()
        {
            _mockRepository.Setup(repo => repo.CreateTemplateAsync(It.IsAny<HTMLTemplate>()))
                .ReturnsAsync(Result.Success(201));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test name",
                HTMLContent = "<html></html>"
            };

            var result = await controller.CreateHTMLTemplate(template);

            result.Should().BeOfType<CreatedResult>();

            var response = result.As<CreatedResult>();

            response.StatusCode.Should().Be(201);
            response.Value.Should().Be("Created");
        }

        [Fact]
        public async Task CreateHTMLTemplate_ShouldReturn400BadRequest_WhenNameIsEmpty()
        {
            HTMLTemplate template = new HTMLTemplate
            {
                Name = string.Empty,
                HTMLContent = "<html></html>"
            };

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.CreateHTMLTemplate(template);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();

            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Name cannot be empty");
        }

        [Fact]
        public async Task CreateHTMLTemplate_ShouldReturn400BadRequest_WhenHTMLContentIsEmpty()
        {
            HTMLTemplate template = new HTMLTemplate
            {
                Name = "Test name",
                HTMLContent = string.Empty
            };

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.CreateHTMLTemplate(template);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();

            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("HTML content cannot be empty");
        }

        [Fact]
        public async Task CreateHTMLTemplate_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            _mockRepository.Setup(repo => repo.CreateTemplateAsync(It.IsAny<HTMLTemplate>()))
                .ReturnsAsync(Result.Failure("An error occurred while creating the template", 500));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test name",
                HTMLContent = "<html></html>"
            };

            var result = await controller.CreateHTMLTemplate(template);

            result.Should().BeOfType<ObjectResult>();
            var response = result.As<ObjectResult>();

            response.StatusCode.Should().Be(500);
            response.Value.Should().Be("An error occurred while creating the template");
        }
        #endregion

        #region GetHTMLTemplateById
        [Fact]
        public async Task GetHTMLTemplateById_ShouldReturnCorrectResponse()
        {
            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test name",
                HTMLContent = "<html></html>"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Success(template, 200));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.GetHTMLTemplateById(template.Id);

            result.Result.Should().BeOfType<OkObjectResult>();

            var response = result.Result.As<OkObjectResult>();

            response.StatusCode.Should().Be(200);
            response.Value.Should().Be(template);
        }

        [Fact]
        public async Task GetHTMLTemplateById_ShouldReturn404NotFound_WhenTemplateDoesNotExist()
        {
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Failure(404, "Template not found"));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.GetHTMLTemplateById(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
            var response = result.Result.As<NotFoundObjectResult>();

            response.StatusCode.Should().Be(404);
            response.Value.Should().Be("Template not found");
        }

        [Fact]
        public async Task GetHTMLTemplateById_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Failure(500, "An error occurred while retrieving the template"));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.GetHTMLTemplateById(Guid.NewGuid());

            result.Result.Should().BeOfType<ObjectResult>();
            var response = result.Result.As<ObjectResult>();

            response.StatusCode.Should().Be(500);
            response.Value.Should().Be("An error occurred while retrieving the template");
        }

        [Fact]
        public async Task GetHTMLTemplateById_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.GetHTMLTemplateById(Guid.Empty);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.Result.As<BadRequestObjectResult>();

            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Id cannot be empty");
        }
        #endregion

        #region GetHTMLTemplates
        [Fact]
        public async Task GetHTMLTemplates_ShouldReturnCorrectResponse()
        {
            List<HTMLTemplate> templates = new List<HTMLTemplate>
            {
                new HTMLTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Test name 1",
                    HTMLContent = "<html></html>"
                },
                new HTMLTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Test name 2",
                    HTMLContent = "<html></html>"
                }
            };

            _mockRepository.Setup(repo => repo.GetAllTemplatesAsync())
                .ReturnsAsync(Result<IEnumerable<HTMLTemplate>>.Success(templates, 200));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.GetHTMlTemplates();

            result.Result.Should().BeOfType<OkObjectResult>();

            var response = result.Result.As<OkObjectResult>();

            response.StatusCode.Should().Be(200);
            response.Value.Should().Be(templates);
        }

        [Fact]
        public async Task GetHTMLTemplates_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            _mockRepository.Setup(repo => repo.GetAllTemplatesAsync())
                .ReturnsAsync(Result<IEnumerable<HTMLTemplate>>.Failure(500, "An error occurred while retrieving the templates"));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.GetHTMlTemplates();

            result.Result.Should().BeOfType<ObjectResult>();
            var response = result.Result.As<ObjectResult>();

            response.StatusCode.Should().Be(500);
            response.Value.Should().Be("An error occurred while retrieving the templates");
        }
        #endregion

        #region DeleteHTMLTemplate
        [Fact]
        public async Task DeleteHTMLTemplate_ShouldReturnCorrectResponse()
        {
            _mockRepository.Setup(repo => repo.DeleteTemplateAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Success(200));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.DeleteHTMLTemplate(Guid.NewGuid());

            result.Should().BeOfType<OkResult>();

            var response = result.As<OkResult>();

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteHTMLTemplate_ShouldReturn404NotFound_WhenTemplateDoesNotExist()
        {
            _mockRepository.Setup(repo => repo.DeleteTemplateAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Failure("Template not found", 404));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.DeleteHTMLTemplate(Guid.NewGuid());

            result.Should().BeOfType<NotFoundObjectResult>();
            var response = result.As<NotFoundObjectResult>();

            response.StatusCode.Should().Be(404);
            response.Value.Should().Be("Template not found");
        }

        [Fact]
        public async Task DeleteHTMLTemplate_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            _mockRepository.Setup(repo => repo.DeleteTemplateAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Failure("An error occurred while deleting the template", 500));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.DeleteHTMLTemplate(Guid.NewGuid());

            result.Should().BeOfType<ObjectResult>();
            var response = result.As<ObjectResult>();

            response.StatusCode.Should().Be(500);
            response.Value.Should().Be("An error occurred while deleting the template");
        }

        [Fact]
        public async Task DeleteHTMLTemplate_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.DeleteHTMLTemplate(Guid.Empty);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();
            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Id cannot be empty");
        }
        #endregion

        #region UpdateHTMLTemplate
        [Fact]
        public async Task UpdateHTMLTemplate_ShouldReturnCorrectResponse()
        {
            _mockRepository.Setup(repo => repo.UpdateTemplateAsync(It.IsAny<HTMLTemplate>()))
                .ReturnsAsync(Result.Success(200));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test name",
                HTMLContent = "<html></html>"
            };

            var result = await controller.UpdateHTMLTemplate(template);

            result.Should().BeOfType<OkResult>();

            var response = result.As<OkResult>();

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateHTMLTemplate_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.Empty,
                Name = "Test name",
                HTMLContent = "<html></html>"
            };

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);
            var result = await controller.UpdateHTMLTemplate(template);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();
            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Id cannot be empty");
        }

        [Fact]
        public async Task UpdateHTMLTemplate_ShouldReturn400BadRequest_WhenNameIsEmpty()
        {
            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                HTMLContent = "<html></html>"
            };

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.UpdateHTMLTemplate(template);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();

            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Name cannot be empty");
        }

        [Fact]
        public async Task UpdateHTMLTemplate_ShouldReturn400BadRequest_WhenHTMLContentIsEmpty()
        {
            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test name",
                HTMLContent = string.Empty
            };

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.UpdateHTMLTemplate(template);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();

            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("HTML content cannot be empty");
        }

        [Fact]
        public async Task UpdateHTMLTemplate_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            _mockRepository.Setup(repo => repo.UpdateTemplateAsync(It.IsAny<HTMLTemplate>()))
                .ReturnsAsync(Result.Failure("An error occurred while updating the template", 500));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test name",
                HTMLContent = "<html></html>"
            };

            var result = await controller.UpdateHTMLTemplate(template);

            result.Should().BeOfType<ObjectResult>();
            var response = result.As<ObjectResult>();

            response.StatusCode.Should().Be(500);
            response.Value.Should().Be("An error occurred while updating the template");
        }
        #endregion

        #region CreatePdfFile
        [Fact]
        public async Task CreatePdfFile_ShouldReturnCorrectResponse()
        {
            var pdfContent = (new byte[] { 37, 80, 68, 70, 45 }, "Test name");

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Success(new HTMLTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Test name",
                    HTMLContent = "<html><body><h1>{{Title}}</h1></body></html>"
                }, 200));

            _mockPdfService.Setup(service => service.ConvertHtmlToPdfAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(Result<(byte[], string)>.Success(pdfContent, 200));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var content = new Dictionary<string, string>
            {
                { "Title", "Hello, World!" }
            };

            var result = await controller.CreatePdfFile(Guid.NewGuid(), content, _mockPdfService.Object);

            result.Should().BeOfType<FileContentResult>();

            var response = result.As<FileContentResult>();

            response.Should().NotBeNull();
            response.FileContents.Should().BeEquivalentTo(pdfContent.Item1);
            response.ContentType.Should().Be("application/pdf");
            response.FileDownloadName.Should().Be("Test name.pdf");
        }

        [Fact]
        public async Task CreatePdfFile_ShouldReturn404NotFound_WhenTemplateDoesNotExist()
        {
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Failure(404, "Template not found"));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            _mockPdfService.Setup(service => service.ConvertHtmlToPdfAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(Result<(byte[], string)>.Failure(404, "Template not found"));

            var result = await controller.CreatePdfFile(Guid.NewGuid(), new Dictionary<string, string>(), _mockPdfService.Object);

            result.Should().BeOfType<NotFoundObjectResult>();
            var response = result.As<NotFoundObjectResult>();

            response.StatusCode.Should().Be(404);
            response.Value.Should().Be("Template not found");
        }

        [Fact]
        public async Task CreatePdfFile_ShouldReturn500InternalServerError_WhenPdfServiceFails()
        {
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<HTMLTemplate>.Success(new HTMLTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Test name",
                    HTMLContent = "<html><body><h1>{{Title}}</h1></body></html>"
                }, 200));

            _mockPdfService.Setup(service => service.ConvertHtmlToPdfAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(Result<(byte[], string)>.Failure(500, "An error occurred while generating the PDF"));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.CreatePdfFile(Guid.NewGuid(), new Dictionary<string, string>(), _mockPdfService.Object);

            result.Should().BeOfType<ObjectResult>();
            var response = result.As<ObjectResult>();

            response.StatusCode.Should().Be(500);
            response.Value.Should().Be("An error occurred while generating the PDF");
        }

        [Fact]
        public async Task CreatePdfFile_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.CreatePdfFile(Guid.Empty, new Dictionary<string, string>(), _mockPdfService.Object);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();

            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Template ID cannot be empty");
        }

        [Fact]
        public async Task CreatePdfFile_ShouldReturn400BadRequest_WhenContentIsNull()
        {
            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.CreatePdfFile(Guid.NewGuid(), null, _mockPdfService.Object);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();

            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Content cannot be null");
        }

        [Fact]
        public async Task CreatePdfFile_ShouldReturn400BadRequest_WhenSomevalueIsMissingInContent()
        {
            _mockPdfService.Setup(service => service.ConvertHtmlToPdfAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(Result<(byte[], string)>.Failure(400, "Missing value for placeholder World"));

            var controller = new HTMLTemplateController(_mockLogger.Object, _mockRepository.Object);

            var result = await controller.CreatePdfFile(Guid.NewGuid(), new Dictionary<string, string>(), _mockPdfService.Object);

            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result.As<BadRequestObjectResult>();
            response.StatusCode.Should().Be(400);
            response.Value.Should().Be("Missing value for placeholder World");
        }
        #endregion
    }
}
