using CommissionCalculator.Core.Interfaces;
using CommissionCalculator.Core.Services;
using CommissionCalculator.Core.Strategies;
using CommissionCalculator.Infrastructure.Repositories;

namespace CommissionCalculator.Api.Configurations
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<ICommissionService, CommissionService>();

            // Register the factory
            services.AddSingleton<CommissionCalculatorFactory>();

            // Register repositories
            services.AddScoped<ICommissionRepository, CommissionRepository>();
        }
    }
}
