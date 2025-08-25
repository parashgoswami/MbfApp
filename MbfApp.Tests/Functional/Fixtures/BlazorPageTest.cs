using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace MbfApp.Tests.Functional.Fixtures;

public class BlazorPageTest : BrowserTest
{
    private BlazorAppFactory? host;
    private IPage? page;
    private IBrowserContext? context;

    public IBrowserContext Context => context ?? throw new InvalidOperationException("Setup has not been run.");

    public IPage Page => page ?? throw new InvalidOperationException("Setup has not been run.");

    public BlazorAppFactory Host => host ?? throw new InvalidOperationException("Setup has not been run.");

    public virtual BrowserNewContextOptions? ContextOptions() => null;

    protected virtual void ConfigureWebHost(IWebHostBuilder builder) { }

    public override async Task InitializeAsync()
    {
        host = new BlazorAppFactory(ConfigureWebHost);
        await host.StartAsync();
        await base.InitializeAsync();

        var options = ContextOptions() ?? new BrowserNewContextOptions();
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
