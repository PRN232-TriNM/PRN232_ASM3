using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVCS.TriNM.Repositories.Repository
{
    public class ChargerTriNMRepository : GenericRepository<ChargerTriNm>, IChargerTriNMRepository
    {
        public ChargerTriNMRepository(EVChargingDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChargerTriNm>> GetByStationIdAsync(int stationTriNMId)
        {
            return await _context.ChargerTriNMs
                .Where(c => c.StationTriNmid == stationTriNMId)
                .ToListAsync();
        }
    }

    public interface IChargerTriNMRepository : IGenericRepository<ChargerTriNm>
    {
        Task<IEnumerable<ChargerTriNm>> GetByStationIdAsync(int stationTriNMId);
    }
}
