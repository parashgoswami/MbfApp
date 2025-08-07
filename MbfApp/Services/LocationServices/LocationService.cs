using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos.Locations;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Services;

public class LocationService(AppDbContext _context) : ILocationService
{
    public async Task CreateLocation(LocationRequestDto request)
    {
        var locationExists = await _context.Set<Location>()
            .AnyAsync(a => a.Name.ToUpper() == request.Name.ToUpper());

        if (locationExists)
            throw new InvalidOperationException("Location name must be unique.");

        var location = new Location
        {
            Name = request.Name.ToUpper()
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location != null)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);

        return location != null
            ? new LocationDto { Id = location.Id, Name = location.Name }
            : null;
    }

    public async Task<List<LocationDto>> GetLocationListAync()
    {
        return await _context.Locations
            .Select(l => new LocationDto { Id = l.Id, Name = l.Name })
            .ToListAsync();
    }

    public async Task UpdateLocation(int id, LocationRequestDto request)
    {
        var location = await _context.Locations.FindAsync(id);
        
        if (location == null)
            throw new InvalidOperationException("Location not found.");

        if (location.Name.ToUpper() != request.Name.ToUpper())
        {
            var exists = _context.Set<Location>()
                .Any(a => a.Name.ToUpper() == request.Name.ToUpper());

            if (exists)
                throw new InvalidOperationException("Location name must be unique.");
        }

        location.Name = request.Name.ToUpper();
        await _context.SaveChangesAsync();
    }
}