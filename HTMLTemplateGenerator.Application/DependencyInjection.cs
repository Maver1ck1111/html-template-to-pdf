using HTMLTemplateGenerator.Application.Services;
using HTMLTemplateGenerator.Application.ServicesContracts;
using Microsoft.Extensions.DependencyInjection;


namespace HTMLTemplateGenerator.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection ApplicationDependencies(this IServiceCollection service)
        {
            service.AddSingleton<IPdfService, PdfService>();
            return service;
        }
    }
}
