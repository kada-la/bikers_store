using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Infrastructure.Persistence;

namespace SalesManagementSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure the database connection string from environment variables
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<SalesDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register Unit of Work & Repositories (Scoped lifetime)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Configure Unit4 ERP Options and Typed HttpClient
        services.Configure<SalesManagementSystem.Application.Common.Unit4Settings>(
            configuration.GetSection(SalesManagementSystem.Application.Common.Unit4Settings.SectionName));

        services.AddHttpClient<IUnit4SupplierClient, SalesManagementSystem.Infrastructure.ExternalServices.Unit4SupplierClient>();

        return services;
    }
}
