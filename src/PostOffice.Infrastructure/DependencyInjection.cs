using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostOffice.Application.Abstractions;
using PostOffice.Infrastructure.Persistence;

namespace PostOffice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostOfficeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostOfficeDb")));

        services.AddScoped<IPostOfficeRepository, PostOfficeRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        return services;
    }
}
