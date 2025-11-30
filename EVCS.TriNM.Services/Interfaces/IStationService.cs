using EVCS.TriNM.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface IStationService
    {
        Task<IEnumerable<StationTriNm>> GetAllStationsAsync();
        Task<StationTriNm?> GetStationByIdAsync(int id);
        Task<IEnumerable<StationTriNm>> GetActiveStationsAsync();
        Task<StationTriNm?> GetStationWithChargersAsync(int stationTriNMId);
        Task<IEnumerable<StationTriNm>> GetStationsByLocationAsync(string location);
        Task<IEnumerable<StationTriNm>> SearchStationsAsync(string? name, string? location, bool? isActive);
        Task<(IEnumerable<StationTriNm> Items, int TotalCount)> SearchStationsWithPaginationAsync(
            string? name, string? location, bool? isActive, int pageNumber, int pageSize);
        Task<(IEnumerable<StationTriNm> Items, int TotalCount)> GetStationsWithPaginationAsync(int pageNumber, int pageSize);
        Task<StationTriNm> CreateStationAsync(StationTriNm station);
        Task<StationTriNm?> UpdateStationAsync(StationTriNm station);
        Task<bool> DeleteStationAsync(int id, bool hardDelete = false);
        Task<bool> ActivateStationAsync(int id);
    }
}

