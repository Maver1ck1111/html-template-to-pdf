using HTMLTemlateGenerator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemplateGenerator.Application.RepositoriesContracts
{
    public interface IHTMLRepository
    {
        Task<Result> CreateTemplateAsync(HTMLTemplate template);
        Task<Result> UpdateTemplateAsync(HTMLTemplate template);
        Task<Result> DeleteTemplateAsync(Guid id);
        Task<Result<HTMLTemplate>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<HTMLTemplate>>> GetAllTemplatesAsync();
    }
}
