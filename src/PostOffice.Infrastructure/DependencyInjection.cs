using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PostOffice.Application.Abstractions;
using PostOffice.Application.Interface;
using PostOffice.Infrastructure.Persistence;

namespace PostOffice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb") ?? configuration["MongoDb:ConnectionString"];
        var databaseName = configuration["MongoDb:Database"] ?? configuration["MongoDbDatabase"] ?? "PostOfficeDb";

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp => new PostOfficeDbContext(sp.GetRequiredService<IMongoClient>(), databaseName));

        services.AddScoped<IPostOfficeRepository, PostOfficeRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        return services;
    }
}
