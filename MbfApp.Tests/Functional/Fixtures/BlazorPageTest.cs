using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace MbfApp.Tests.Functional.Fixtures;

public abstract class BlazorPageTest : BrowserTest
{
    private BlazorAppFactory? host;
    private IPage? page;
    private IBrowserContext? context;
    
    public IBrowserContext Context => context ?? throw new InvalidOperationException("Setup has not been run.");

    public IPage Page => page ?? throw new InvalidOperationException("Setup has not been run.");

    public BlazorAppFactory Host => host ?? throw new InvalidOperationException("Setup has not been run.");

    protected abstract string ConnectionString { get; }

    protected virtual void ConfigureWebHost(IWebHostBuilder builder) { }

    public override async Task InitializeAsync()
    {
        if(ConnectionString.IsNullOrEmpty())
            throw new InvalidOperationException("DB connection string not set.");

        host = new BlazorAppFactory(ConnectionString, ConfigureWebHost);
        await host.StartAsync();
        await base.InitializeAsync();

        var options = new BrowserNewContextOptions();
        options.BaseURL = Host.ServerAddress;
        options.IgnoreHTTPSErrors = true;

        context = await NewContext(options);
        page = await Context.NewPageAsync();
    }

    public override async Task DisposeAsync()
    {
        if (page is not null)
        {
            await page.CloseAsync();
        }

        if (context is not null)
        {
            await context.DisposeAsync();
        }

        if (host is not null)
        {
            await host.DisposeAsync();
            host = null;
        }
    }
}
