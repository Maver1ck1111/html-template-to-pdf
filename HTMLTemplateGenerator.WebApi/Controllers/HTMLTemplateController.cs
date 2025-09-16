using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
using HTMLTemplateGenerator.Application.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTMLTemplateGenerator.WebApi.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class HTMLTemplateController : ControllerBase
    {
        private readonly ILogger<HTMLTemplateController> _logger;
        private readonly IHTMLRepository _htmlRepository;
        public HTMLTemplateController(ILogger<HTMLTemplateController> logger, IHTMLRepository htmlRepository)
        {
            _logger = logger;
            _htmlRepository = htmlRepository;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<HTMLTemplate>> GetHTMLTemplateById(Guid id)
        {
            if(id == Guid.Empty)
            {
                _logger.LogError("Id is empty");
                return BadRequest("Id cannot be empty");
            }

            Result<HTMLTemplate> result = await _htmlRepository.GetByIdAsync(id);

            if(result.StatusCode == 404)
            {
                _logger.LogError("Template not found");
                return NotFound("Template not found");
            }

            if(!result.IsSuccess)
            {
                _logger.LogError(result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Template retrieved successfully");
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HTMLTemplate>>> GetHTMlTemplates()
        {
            Result<IEnumerable<HTMLTemplate>> result = await _htmlRepository.GetAllTemplatesAsync();

            if(!result.IsSuccess)
            {
                _logger.LogError(result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Templates retrieved successfully");
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHTMLTemplate(HTMLTemplate htmlTemplate)
        {
            if(htmlTemplate == null)
            {
                _logger.LogError("Template is null");
                return BadRequest("Template cannot be null");
            }

            if(string.IsNullOrEmpty(htmlTemplate.Name))
            {
                _logger.LogError("Template name is empty");
                return BadRequest("Name cannot be empty");
            }

            if(string.IsNullOrEmpty(htmlTemplate.HTMLContent))
            {
                _logger.LogError("Template content is empty");
                return BadRequest("HTML content cannot be empty");
            }

            Result result = await _htmlRepository.CreateTemplateAsync(htmlTemplate);

            if(!result.IsSuccess)
            {
                _logger.LogError(result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Template created successfully");
            return Created(string.Empty, "Created");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteHTMLTemplate(Guid id)
        {
            if(id == Guid.Empty)
            {
                _logger.LogError("Id ca not be empty");
                return BadRequest("Id cannot be empty");
            }

            Result result = await _htmlRepository.DeleteTemplateAsync(id);

            if(result.StatusCode == 404)
            {
                _logger.LogError("Template not found");
                return NotFound("Template not found");
            }

            if (!result.IsSuccess)
            {
                _logger.LogError(result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHTMLTemplate(HTMLTemplate htmlTemplate)
        {
            if(htmlTemplate == null)
            {
                _logger.LogError("Template is null");
                return BadRequest("Template cannot be null");
            }

            if(htmlTemplate.Id == Guid.Empty)
            {
               _logger.LogError("Template id is empty");
                return BadRequest("Id cannot be empty");
            }

            if(string.IsNullOrEmpty(htmlTemplate.Name))
            {
                _logger.LogError("Template name is empty");
                return BadRequest("Name cannot be empty");
            }

            if(string.IsNullOrEmpty(htmlTemplate.HTMLContent))
            {
                _logger.LogError("Template content is empty");
                return BadRequest("HTML content cannot be empty");
            }

            Result result = await _htmlRepository.UpdateTemplateAsync(htmlTemplate);

            if(result.StatusCode == 404)
            {
                _logger.LogError("Template not found");
                return NotFound("Template not found");
            }

            if (!result.IsSuccess)
            {
                _logger.LogError(result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            _logger.LogInformation("Template updated successfully");
            return Ok();
        }

        [HttpPost("createpdf/{id:guid}")]
        public async Task<IActionResult> CreatePdfFile([FromRoute] Guid id, [FromBody] Dictionary<string, string> htmlValues, [FromServices] IPdfService pdfService)
        {
            if(id == Guid.Empty)
            {
                _logger.LogError("Id is empty");
                return BadRequest("Template ID cannot be empty");
            }

            if(htmlValues == null)
            {
                _logger.LogError("Content is null");
                return BadRequest("Content cannot be null");
            }

            Result<(byte[], string)> result = await pdfService.ConvertHtmlToPdfAsync(id, htmlValues);

            if(result.StatusCode == 404)
            {
                _logger.LogError("Template not found");
                return NotFound("Template not found");
            }

            if(result.StatusCode == 400)
            {
                _logger.LogError(result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }   

            if(!result.IsSuccess)
            {
                _logger.LogError(result.ErrorMessage);
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            var (pdfBytes, fileName) = result.Value;

            _logger.LogInformation("File created successfully");
            return File(pdfBytes, "application/pdf", $"{fileName}.pdf");
        }
    }
}
