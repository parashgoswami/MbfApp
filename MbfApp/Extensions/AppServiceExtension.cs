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
        services.AddScoped<IMemberLedgerService, MemberLedgerService>();
        services.AddScoped<ICreateMemberService,CreateMemberService>();
        services.AddScoped<IFinYearService, FinYearService>();
        services.AddScoped<IVoucherNumberService, VoucherNumberService>();

        return services;
    }
}
