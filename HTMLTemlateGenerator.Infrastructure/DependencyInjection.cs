using HTMLTemplateGenerator.Application.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemlateGenerator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection InfrastructureDependencies(this IServiceCollection service)
        {
            string connectionString = "";
            connectionString += "Host=" + Environment.GetEnvironmentVariable("DbHostName")+ "; ";
            connectionString += "Port=" + Environment.GetEnvironmentVariable("DbPort")+ "; ";
            connectionString += "Database=" + Environment.GetEnvironmentVariable("DbName")+ "; ";
            connectionString += "Username=" + Environment.GetEnvironmentVariable("DbUserName")+ "; ";
            connectionString += "Password=" + Environment.GetEnvironmentVariable("DbPassword")+ "; ";

            service.AddDbContext<HTMLDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            service.AddScoped<IHTMLRepository, HTMLRepository>();

            return service;
        }
    }
}
