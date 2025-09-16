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
        public async Task<Result<(byte[], string)>> ConvertHtmlToPdfAsync(Guid templateId)
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

            byte[] pdfBytes;

            try
            {
                IBrowser browser = await GetBrowserAsync();
                using var page = await browser.NewPageAsync();
                await page.SetContentAsync(templateResult.Value.HTMLContent);

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
    }
}
