using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
using HTMLTemplateGenerator.Application.ServicesContracts;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemplateGenerator.Application.Services
{
    public class PdfService : IPdfService
    {
        private readonly ILogger<PdfService> _logger;
        private readonly IHTMLRepository _htmlRepository;
        private IBrowser? _browser;

        public PdfService(ILogger<PdfService> logger, IHTMLRepository hTMLRepository)
        {
            _logger = logger;
            _htmlRepository = hTMLRepository;
        }
        public async Task<Result<(byte[], string)>> ConvertHtmlToPdfAsync(Guid templateId, Dictionary<string, string> content)
        {
            if(templateId == Guid.Empty)
            {
                _logger.LogError("Template ID is empty");
                return Result<(byte[], string)>.Failure(400, "Template ID cannot be empty");
            }

            Result<HTMLTemplate> templateResult = await _htmlRepository.GetByIdAsync(templateId);

            if(!templateResult.IsSuccess)
            {
                _logger.LogError(templateResult.ErrorMessage);
                return Result<(byte[], string)>.Failure(templateResult.StatusCode, templateResult.ErrorMessage);
            }

            Result<string> addingResult =  SetValuesIntoHTML(templateResult.Value.HTMLContent, content);

            if(!addingResult.IsSuccess)
            {
                _logger.LogError(addingResult.ErrorMessage);
                return Result<(byte[], string)>.Failure(addingResult.StatusCode, addingResult.ErrorMessage);
            }

            string html = addingResult.Value;

            byte[] pdfBytes;

            try
            {
                IBrowser browser = await GetBrowserAsync();
                using var page = await browser.NewPageAsync();
                await page.SetContentAsync(html);

                pdfBytes = await page.PdfDataAsync(new PdfOptions { Format = PaperFormat.A4 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the PDF");
                return Result<(byte[], string)>.Failure(500, "An error occurred while generating the PDF");
            }

            _logger.LogInformation("PDF generated successfully");
            return Result<(byte[], string)>.Success((pdfBytes, templateResult.Value.Name), 200);
        }

        private async Task<IBrowser> GetBrowserAsync()
        {
            if (_browser == null)
            {
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();

                _browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            }

            return _browser;
        }

        private Result<string> SetValuesIntoHTML(string html, Dictionary<string, string> content)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(html, @"\{\{(\w+)\}\}");

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string placeholder = match.Groups[1].Value;

                if (content.TryGetValue(placeholder, out var value))
                {
                    html = html.Replace(match.Value, value);
                }
                else
                {
                    return Result<string>.Failure(400, $"Missing value for placeholder '{placeholder}'");
                }
            }

            return Result<string>.Success(html, 200);
        }
    }
}
