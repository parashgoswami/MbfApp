using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MbfApp.Data;
using MbfApp.Tests.Functional.Utils;

namespace MbfApp.Tests.Functional.Fixtures;

public sealed class BlazorAppFactory(
    string connectionString,
    Action<IWebHostBuilder>? configureWebHost = null) : WebApplicationFactory<Program>
{
    private IHost? host;

    public override IServiceProvider Services
        => host?.Services
        ?? throw new InvalidOperationException("Call StartAsync() first to start host.");

    public string ServerAddress => host is not null
        ? ClientOptions.BaseAddress.ToString()
        : throw new InvalidOperationException("Call StartAsync() first to start host.");

    public async Task StartAsync()
    {
        // Triggers CreateHost() getting called.
        _ = base.Services;

        Debug.Assert(host is not null);

        // Spins the host app up.
        await host.StartAsync();

        // Extract the selected dynamic port out of the Kestrel server
        // and assign it onto the client options for convenience so it
        // "just works" as otherwise it'll be the default http://localhost
        // URL, which won't route to the Kestrel-hosted HTTP server.
        var server = host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();

        ClientOptions.BaseAddress = addresses!.Addresses
            .Select(x => x.Replace("127.0.0.1", "localhost", StringComparison.Ordinal))
            .Select(x => new Uri(x))
            .Last();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureWebHost(webHostBuilder =>
        {
            configureWebHost?.Invoke(webHostBuilder);
            webHostBuilder.UseKestrel();
            webHostBuilder.UseUrls("https://127.0.0.1:0");

            webHostBuilder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<AppDbContext>();

                services.AddDbContextFactory<AppDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                });

                services.EnsureCreated<AppDbContext>();
            });
        });

        host = builder.Build();

        // Hack: return dummy host so that the Program project is not started twice.
        return new DummyHost();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        host?.Dispose();
        GC.SuppressFinalize(this);
    }

    // The DummyHost is returned to avoid the Program project being started twice.
    private sealed class DummyHost : IHost
    {
        public IServiceProvider Services { get; }

        public DummyHost()
        {
            Services = new ServiceCollection()
                .AddSingleton<IServer>((s) => new TestServer(s))
                .BuildServiceProvider();
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void Dispose()
        {
        }
    }
}