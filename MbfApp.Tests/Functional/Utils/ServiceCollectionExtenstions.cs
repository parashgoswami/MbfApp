using MbfApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MbfApp.Tests.Functional.Utils;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));

        if (descriptor != null) services.Remove(descriptor);
    }

    public static void EnsureCreated<T>(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.Migrate();

        // await AppDbSeed.SeedAdminUserAsync(sp);
        // await AppDbSeed.SeedAccountAsync(sp);
        // await AppDbSeed.SeedLocationAsync(sp);
        // await AppDbSeed.SeedMembersAsync(sp);
    }
}

