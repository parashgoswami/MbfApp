using MbfApp.Services.LoanServices;

namespace MbfApp.Services;

public static class AppServiceDependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {

        builder.Services.AddScoped<ILoanService, LoanService>();        

    }
}
