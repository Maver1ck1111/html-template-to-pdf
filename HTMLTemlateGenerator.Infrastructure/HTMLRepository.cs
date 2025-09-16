using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemlateGenerator.Infrastructure
{
    public class HTMLRepository : IHTMLRepository
    {
        private readonly ILogger<HTMLRepository> _logger;
        private readonly HTMLDbContext _context;
        public HTMLRepository(ILogger<HTMLRepository> logger, HTMLDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<Result> CreateTemplateAsync(HTMLTemplate template)
        {
            if(template == null)
            {
                _logger.LogError("Template is null");
                return Result.Failure("Template cannot be null", 400);
            }

            if(template.Name == string.Empty)
            {
                _logger.LogError("Template name is empty");
                return Result.Failure("Name cannot be empty", 400);
            }

            if(template.HTMLContent == string.Empty)
            {
                _logger.LogError("Template content is empty");
                return Result.Failure("HTML content cannot be empty", 400);
            }

            template.Id = Guid.NewGuid();

            try
            {
                await _context.HTMLTemplates.AddAsync(template);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the template");
                return Result.Failure("An error occurred while creating the template", 500);
            }

            _logger.LogInformation("Template created successfully");
            return Result.Success(201);
        }

        public async Task<Result> DeleteTemplateAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                _logger.LogError("Template id is empty");
                return Result.Failure("Id cannot be empty", 400);
            }

            try
            {
                HTMLTemplate? template = await _context.HTMLTemplates.FindAsync(id);
                if(template == null)
                {
                    _logger.LogError("Can not find account in DeleteTemplateAsync");
                    return Result.Failure("Template not found", 404);
                }

                _context.HTMLTemplates.Remove(template);
                await _context.SaveChangesAsync();
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the template");
                return Result.Failure("An error occurred while deleting the template", 500);
            }

            _logger.LogInformation("Template deleted successfully");
            return Result.Success(200);
        }

        public async Task<Result<IEnumerable<HTMLTemplate>>> GetAllTemplatesAsync()
        {
            IEnumerable<HTMLTemplate> templates;
            try
            {
                templates = await _context.HTMLTemplates.ToListAsync();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving templates");
                return Result<IEnumerable<HTMLTemplate>>.Failure(500, "An error occurred while retrieving the templates");
            }

            _logger.LogInformation("Templates retrieved successfully");
            return Result<IEnumerable<HTMLTemplate>>.Success(templates, 200);
        }

        public async Task<Result<HTMLTemplate>> GetByIdAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                _logger.LogError("Template id is empty");
                return Result<HTMLTemplate>.Failure(400, "Id cannot be empty" );
            }

            HTMLTemplate? template;

            try
            {
                template = await _context.HTMLTemplates.FindAsync(id);
                if(template == null)
                {
                    _logger.LogError("Can not find account in GetByIdAsync");
                    return Result<HTMLTemplate>.Failure(404, "Template not found");
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving template");
                return Result<HTMLTemplate>.Failure(500, "An error occurred while retrieving the template");
            }

            _logger.LogInformation("Template retrieved successfully");
            return Result<HTMLTemplate>.Success(template, 200);
        }

        public async Task<Result> UpdateTemplateAsync(HTMLTemplate template)
        {
            if(template == null)
            {
                _logger.LogError("Template is null");
                return Result.Failure("Template cannot be null", 400);
            }

            if (template.Name == string.Empty)
            {
                _logger.LogError("Template name is empty");
                return Result.Failure("Name cannot be empty", 400);
            }

            if (template.HTMLContent == string.Empty)
            {
                _logger.LogError("Template content is empty");
                return Result.Failure("HTML content cannot be empty", 400);
            }

            HTMLTemplate? currentTemplate;

            try
            {
                currentTemplate = await _context.HTMLTemplates.FindAsync(template.Id);
                if (currentTemplate == null) 
                {
                    _logger.LogError("Can not find account in UpdateTemplateAsync");
                    return Result.Failure("Template not found", 404);
                }

                currentTemplate.Name = template.Name;
                currentTemplate.HTMLContent = template.HTMLContent;
                _context.HTMLTemplates.Update(currentTemplate);
                await _context.SaveChangesAsync();

            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating template");
                return Result.Failure("An error occurred while updating the template", 500);
            }

            _logger.LogInformation("Template updated successfully");
            return Result.Success(200);
        }
    }
}
