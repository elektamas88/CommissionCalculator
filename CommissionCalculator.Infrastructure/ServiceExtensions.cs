using CommissionCalculator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommissionCalculator.Api.Configurations
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseInMemoryDatabase("CommissionDB"));
        }
    }
}
