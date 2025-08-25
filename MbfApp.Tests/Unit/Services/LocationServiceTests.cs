using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos.Locations;
using MbfApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Tests.Unit.Services;

public class LocationServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateLocation_ShouldAddLocation_WhenNameIsUnique()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var request = new LocationRequestDto { Name = "Test Location" };

        // Act
        var service = new LocationService(context);
        await service.CreateLocation(request);

        // Assert
        var location = await context.Locations.SingleOrDefaultAsync(l => l.Name == "TEST LOCATION");
        Assert.NotNull(location);
        Assert.Equal("TEST LOCATION", location.Name);
    }

    [Fact]
    public async Task CreateLocation_ShouldThrowInvalidOperationException_WhenNameExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Locations.Add(new Location { Name = "EXISTING LOCATION" });
        await context.SaveChangesAsync();

        var service = new LocationService(context);
        var request = new LocationRequestDto { Name = "Existing Location" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateLocation(request));
        Assert.Equal("Location name must be unique.", exception.Message);
    }

    [Fact]
    public async Task GetLocationByIdAsync_ShouldReturnLocation_WhenLocationExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Locations.Add(new Location { Id = 1, Name = "Test Location" });
        await context.SaveChangesAsync();
        var service = new LocationService(context);

        // Act
        var result = await service.GetLocationByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Location", result.Name);
    }

    [Fact]
    public async Task GetLocationByIdAsync_ShouldReturnNull_WhenLocationDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new LocationService(context);

        // Act
        var result = await service.GetLocationByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLocationListAync_ShouldReturnAllLocations()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Locations.Add(new Location { Name = "Location 1" });
        context.Locations.Add(new Location { Name = "Location 2" });
        await context.SaveChangesAsync();

        var service = new LocationService(context);

        // Act
        var result = await service.GetLocationListAync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateLocation_ShouldUpdateLocation_WhenDataIsValid()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Locations.Add(new Location { Id = 1, Name = "Old Name" });
        await context.SaveChangesAsync();

        var service = new LocationService(context);
        var request = new LocationRequestDto { Name = "New Name" };

        // Act
        await service.UpdateLocation(1, request);

        // Assert
        var location = await context.Locations.FindAsync(1);
        Assert.NotNull(location);
        Assert.Equal("NEW NAME", location.Name);
    }

    [Fact]
    public async Task UpdateLocation_ShouldThrowInvalidOperationException_WhenLocationNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        var service = new LocationService(context);
        var request = new LocationRequestDto { Name = "New Name" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateLocation(99, request));
        Assert.Equal("Location not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateLocation_ShouldThrowInvalidOperationException_WhenNameExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Locations.Add(new Location { Id = 1, Name = "OLD NAME" });
        context.Locations.Add(new Location { Id = 2, Name = "EXISTING NAME" });
        await context.SaveChangesAsync();

        var service = new LocationService(context);
        var request = new LocationRequestDto { Name = "Existing Name" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateLocation(1, request));
        Assert.Equal("Location name must be unique.", exception.Message);
    }
    
    [Fact]
    public async Task DeleteLocation_ShouldRemoveLocation_WhenLocationExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Locations.Add(new Location { Id = 1, Name = "Test Location" });
        await context.SaveChangesAsync();

        // Act
        var service = new LocationService(context);
        await service.DeleteLocation(1);

        // Assert
        var exists = await context.Locations.AnyAsync();
        Assert.False(exists);
    }
}
