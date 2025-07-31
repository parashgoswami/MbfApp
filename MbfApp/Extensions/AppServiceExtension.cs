using MbfApp.Services.AccountCodeService;
using MbfApp.Services.LoanServices;
using MbfApp.Services.MemberServices;

namespace MbfApp.Extensions;

public static class AppServiceExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IAccountCodeService, AccountCodeService>();

        return services;
    }
}
