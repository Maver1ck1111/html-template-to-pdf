using HTMLTemlateGenerator.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemlateGenerator.Infrastructure
{
    public class HTMLDbContext: DbContext
    {
        public HTMLDbContext(DbContextOptions<HTMLDbContext> options): base(options)
        { }

        public DbSet<HTMLTemplate> HTMLTemplates { get; set; } = null!;
    }
}
