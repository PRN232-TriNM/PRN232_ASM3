using EVCS.TriNM.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface IChargerService
    {
        Task<IEnumerable<ChargerTriNm>> GetAllChargersAsync();
        Task<ChargerTriNm?> GetChargerByIdAsync(int id);
        Task<IEnumerable<ChargerTriNm>> GetChargersByStationIdAsync(int stationId);
        Task<IEnumerable<ChargerTriNm>> GetAvailableChargersAsync();
        Task<ChargerTriNm> CreateChargerAsync(ChargerTriNm charger);
        Task<bool> UpdateChargerAsync(ChargerTriNm charger);
        Task<bool> DeleteChargerAsync(int id);
    }
}

