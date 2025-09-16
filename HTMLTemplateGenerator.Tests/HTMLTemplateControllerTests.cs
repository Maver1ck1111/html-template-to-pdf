using FluentAssertions;
using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
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
    }
}
