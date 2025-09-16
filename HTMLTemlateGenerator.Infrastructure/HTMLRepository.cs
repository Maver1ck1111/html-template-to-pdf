using HTMLTemlateGenerator.Domain;
using HTMLTemplateGenerator.Application;
using HTMLTemplateGenerator.Application.RepositoriesContracts;
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
        public Task<Result> CreateTemplateAsync(HTMLTemplate template)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteTemplateAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<HTMLTemplate>>> GetAllTemplatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<HTMLTemplate>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateTemplateAsync(HTMLTemplate template)
        {
            throw new NotImplementedException();
        }
    }
}
