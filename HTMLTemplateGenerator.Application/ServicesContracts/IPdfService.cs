using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemplateGenerator.Application.ServicesContracts
{
    public interface IPdfService
    {
        Task<Result<(byte[], string)>> ConvertHtmlToPdfAsync(Guid templateId, Dictionary<string, string> content);
    }
}
