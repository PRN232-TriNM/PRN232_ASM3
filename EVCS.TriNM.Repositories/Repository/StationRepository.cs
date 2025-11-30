using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCS.TriNM.Repositories.Repository
{
    public class StationTriNMRepository : GenericRepository<StationTriNm>
    {
        public StationTriNMRepository(EVChargingDBContext context) : base(context)
        {
        }

        public async Task<List<StationTriNm>> GetActiveStationsAsync()
        {
            return await _context.StationTriNMs
                .Where(s => s.IsActive == true)
                .Include(s => s.ChargerTriNms)
                .ToListAsync();
        }

        public async Task<StationTriNm?> GetStationWithChargersAsync(int stationTriNMId)
        {
            return await _context.StationTriNMs
                .Include(s => s.ChargerTriNms)
                .FirstOrDefaultAsync(s => s.StationTriNmid == stationTriNMId);
        }

        public async Task<List<StationTriNm>> GetStationsByLocationAsync(string location)
        {
            return await _context.StationTriNMs
                .Where(s => s.Address.Contains(location) && s.IsActive == true)
                .ToListAsync();
        }

        public async Task<StationTriNm?> GetStationByCodeAsync(string stationCode)
        {
            return await _context.StationTriNMs
                .FirstOrDefaultAsync(s => s.StationTriNmcode == stationCode);
        }

        public async Task<List<StationTriNm>> SearchStationsAsync(string? name, string? location, bool? isActive)
        {
            var query = _context.StationTriNMs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.StationTriNmname.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(s => s.Address.Contains(location) || 
                                         (s.City != null && s.City.Contains(location)) ||
                                         (s.Province != null && s.Province.Contains(location)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<(List<StationTriNm> Items, int TotalCount)> SearchStationsWithPaginationAsync(
            string? name, string? location, bool? isActive, int pageNumber, int pageSize)
        {
            var query = _context.StationTriNMs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.StationTriNmname.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(s => s.Address.Contains(location) || 
                                         (s.City != null && s.City.Contains(location)) ||
                                         (s.Province != null && s.Province.Contains(location)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
