using FluentAssertions;
using HTMLTemlateGenerator.Domain;
using HTMLTemlateGenerator.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HTMLTemplateGenerator.Tests
{
    public class HTMLRepositoryTests
    {
        private readonly Mock<ILogger<HTMLRepository>> _mockLogger = new Mock<ILogger<HTMLRepository>>();

        private HTMLDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<HTMLDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new HTMLDbContext(options);
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        #region CreateTemplateAsync Tests
        [Fact]
        public async Task CreateTemplateAsync_ShouldReturnCorrectResponse()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.CreateTemplateAsync(template);

            result.StatusCode.Should().Be(201);

            var createdTemplate = await context.HTMLTemplate.FindAsync(template.Id);

            createdTemplate.Should().NotBeNull();
            createdTemplate.Name.Should().Be(template.Name);
            createdTemplate.HTMLContent.Should().Be(template.HTMLContent);
            createdTemplate.Id.Should().Be(template.Id);
        }

        [Fact]
        public async Task CreateTemplateAsync_ShouldReturn400badRequset_WhenTemplateIsNull()
        {
            var context = GetDbContext();

            HTMLTemplate template = null;

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.CreateTemplateAsync(template);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("Template cannot be null");
        }

        [Fact]
        public async Task CreateTemplateAsync_ShouldReturn400badRequset_WhenHTMLContentIsEmpty()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = string.Empty
            }; ;

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.CreateTemplateAsync(template);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("HTML content cannot be empty");
        }

        [Fact]
        public async Task CreateTemplateAsync_ShouldReturn400badRequset_WhenNameIsEmpty()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            }; ;

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.CreateTemplateAsync(template);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("Name cannot be empty");
        }

        [Fact]
        public async Task CreateTemplateAsync_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            var repository = new HTMLRepository(_mockLogger.Object, null);

            var result = await repository.CreateTemplateAsync(template);

            result.StatusCode.Should().Be(500);
            result.ErrorMessage.Should().Be("An error occurred while creating the template");
        }
        #endregion

        #region GetByIdAsync Tests
        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectResponse()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            context.HTMLTemplate.Add(template);
            await context.SaveChangesAsync();

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.GetByIdAsync(template.Id);

            result.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(template.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn404NotFound_WhenTemplateDoesNotExist()
        {
            var context = GetDbContext();

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.GetByIdAsync(Guid.NewGuid());

            result.StatusCode.Should().Be(404);
            result.ErrorMessage.Should().Be("Template not found");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            var context = GetDbContext();

            var repository = new HTMLRepository(_mockLogger.Object, null);

            var result = await repository.GetByIdAsync(Guid.NewGuid());

            result.StatusCode.Should().Be(500);
            result.ErrorMessage.Should().Be("An error occurred while retrieving the template");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var context = GetDbContext();

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.GetByIdAsync(Guid.Empty);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("Id cannot be empty");
        }

        #endregion

        #region GetAllTemplatesAsync Tests
        [Fact]
        public async Task GetAllTemplatesAsync_ShouldReturnCorrectResponse()
        {
            var context = GetDbContext();

            HTMLTemplate template1 = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template 1",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            HTMLTemplate template2 = new HTMLTemplate 
            {
                Id = Guid.NewGuid(),
                Name = "Test Template 2",
                HTMLContent = "<html><body><h1>Hello, Universe!</h1></body></html>"
            };

            context.HTMLTemplate.Add(template1);
            context.HTMLTemplate.Add(template2);
            await context.SaveChangesAsync();

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.GetAllTemplatesAsync();

            result.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
            result.Value.Count().Should().Be(2);

            foreach (var item in result.Value)
            {
                item.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetAllTemplatesAsync_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            var context = GetDbContext();

            var repository = new HTMLRepository(_mockLogger.Object, null);

            var result = await repository.GetAllTemplatesAsync();

            result.StatusCode.Should().Be(500);
            result.ErrorMessage.Should().Be("An error occurred while retrieving the templates");
        }
        #endregion

        #region DeleteTemplateAsync Tests

        [Fact]
        public async Task DeleteTemplateAsync_ShouldReturnCorrectResponse()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            context.HTMLTemplate.Add(template);
            await context.SaveChangesAsync();

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.DeleteTemplateAsync(template.Id);

            result.StatusCode.Should().Be(200);

            var deletedTemplate = await context.HTMLTemplate.FindAsync(template.Id);

            deletedTemplate.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTemplateAsync_ShouldReturn404NotFound_WhenTemplateDoesNotExist()
        {
            var context = GetDbContext();

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.DeleteTemplateAsync(Guid.NewGuid());

            result.StatusCode.Should().Be(404);
            result.ErrorMessage.Should().Be("Template not found");
        }

        [Fact]
        public async Task DeleteTemplateAsync_ShouldReturn400BadRequest_WhenIdIsEmpty()
        {
            var context = GetDbContext();

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.DeleteTemplateAsync(Guid.Empty);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("Id cannot be empty");
        }

        [Fact]
        public async Task DeleteTemplateAsync_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            var context = GetDbContext();

            var repository = new HTMLRepository(_mockLogger.Object, null);

            var result = await repository.DeleteTemplateAsync(Guid.NewGuid());

            result.StatusCode.Should().Be(500);
            result.ErrorMessage.Should().Be("An error occurred while deleting the template");
        }
        #endregion

        #region UpdateTemplateAsync Tests

        [Fact]
        public async Task UpdateTemplateAsync_ShouldReturnCorrectResponse()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            context.HTMLTemplate.Add(template);
            await context.SaveChangesAsync();

            template.Name = "Updated Template";

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.UpdateTemplateAsync(template);
            result.StatusCode.Should().Be(200);

            var updatedTemplate = await context.HTMLTemplate.FindAsync(template.Id);
            updatedTemplate.Should().NotBeNull();
            updatedTemplate.Name.Should().Be("Updated Template");
        }

        [Fact]
        public async Task UpdateTemplateAsync_ShouldReturn404NotFound_WhenTemplateDoesNotExist()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.UpdateTemplateAsync(template);

            result.StatusCode.Should().Be(404);
            result.ErrorMessage.Should().Be("Template not found");
        }

        [Fact]
        public async Task UpdateTemplateAsync_ShouldReturn400BadRequest_WhenTemplateIsNull()
        {
            var context = GetDbContext();

            HTMLTemplate template = null;

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.UpdateTemplateAsync(template);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("Template cannot be null");
        }

        [Fact]
        public async Task UpdateTemplateAsync_ShouldReturn400BadRequest_WhenHTMLContentIsEmpty()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = string.Empty
            };

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.UpdateTemplateAsync(template);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("HTML content cannot be empty");
        }

        [Fact]
        public async Task UpdateTemplateAsync_ShouldReturn400BadRequest_WhenNameIsEmpty()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            var repository = new HTMLRepository(_mockLogger.Object, context);

            var result = await repository.UpdateTemplateAsync(template);

            result.StatusCode.Should().Be(400);
            result.ErrorMessage.Should().Be("Name cannot be empty");
        }

        [Fact]
        public async Task UpdateTemplateAsync_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            var context = GetDbContext();

            HTMLTemplate template = new HTMLTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test Template",
                HTMLContent = "<html><body><h1>Hello, World!</h1></body></html>"
            };

            var repository = new HTMLRepository(_mockLogger.Object, null);

            var result = await repository.UpdateTemplateAsync(template);

            result.StatusCode.Should().Be(500);
            result.ErrorMessage.Should().Be("An error occurred while updating the template");
        }
        #endregion
    }
}
