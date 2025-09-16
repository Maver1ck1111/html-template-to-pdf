using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
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
            return Ok("aaaa");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HTMLTemplate>>> GetHTMlTemplates()
        {
            return Ok("aaaa");
        }

        [HttpPost]
        public async Task<IActionResult> CreateHTMLTemplate(HTMLTemplate htmlTemplate)
        {
            return Ok("aaaa");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteHTMLTemplate(Guid id)
        {
            return Ok("aaaa");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHTMLTemplate(HTMLTemplate htmlTemplate)
        {
            return Ok("aaaa");
        }
    }
}
