using MbfApp.Data;
using MbfApp.Tests.Functional.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Location = MbfApp.Data.Entities.Location;

namespace MbfApp.Tests.Functional.Pages;

public class LocationPagesTests : BlazorPageTest
{
    [Fact]
    public async Task Location_Create_Works()
    {
        await Page.GotoBlazorServerPageAsync("locations/create");

        var locationName = "Test Location";

        await Page.GetByLabel("Name").FillAsync(locationName);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();

        // Assert
        var newLocation = Page.GetByRole(AriaRole.Cell, new() { Name = locationName });
        await Assertions.Expect(newLocation).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Location_Edit_Works()
    {
        // Arrange
        var initialLocationName = "Location to Edit";
        var updatedLocationName = "Updated Location";

        // Seed a location to edit
        await using var scope = Host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var location = new Location { Name = initialLocationName };
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        // Act
        await Page.GotoBlazorServerPageAsync($"locations/edit?Id={location.Id}");

        await Page.GetByLabel("Location:").FillAsync(updatedLocationName);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        // Assert
        await Assertions.Expect(Page.GetByRole(AriaRole.Cell, new() { Name = updatedLocationName })).ToBeVisibleAsync();
        await Assertions.Expect(Page.GetByText(initialLocationName)).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async Task Locations_Index_Displays_Locations()
    {
        // Arrange
        var locationsToSeed = new List<string> { "Location A", "Location B", "Location C" };

        await using var scope = Host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var locationName in locationsToSeed)
        {
            context.Locations.Add(new Location { Name = locationName });
        }
        await context.SaveChangesAsync();

        // Act
        await Page.GotoBlazorServerPageAsync("/locations");

        // Assert
        foreach (var locationName in locationsToSeed)
        {
            await Assertions.Expect(Page.GetByRole(AriaRole.Cell, new() { Name = locationName })).ToBeVisibleAsync();
        }
    }
}
