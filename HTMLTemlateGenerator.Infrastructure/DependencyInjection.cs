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
            return service;
        }
    }
}
