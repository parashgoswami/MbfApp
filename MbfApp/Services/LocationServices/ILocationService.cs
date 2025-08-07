using MbfApp.Dtos.Locations;

namespace MbfApp.Services;

public interface ILocationService
{
    public Task<List<LocationDto>> GetLocationListAync();
    public Task<LocationDto?> GetLocationByIdAsync(int id);  
    public Task CreateLocation(LocationRequestDto request);
    public Task DeleteLocation(int id);
    public Task UpdateLocation(int id, LocationRequestDto request);
}
