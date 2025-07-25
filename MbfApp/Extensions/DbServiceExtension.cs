using MbfApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Extensions;

public static class DbServiceExtension
{
    public static IServiceCollection AddDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddQuickGridEntityFrameworkAdapter();

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }
}
