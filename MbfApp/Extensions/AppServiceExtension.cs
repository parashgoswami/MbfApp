using MbfApp.Services;

namespace MbfApp.Extensions;

public static class AppServiceExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ILocationService, LocationService>();

        return services;
    }
}
