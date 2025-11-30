using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Implements
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<StationTriNm>> GetAllStationsAsync()
        {
            return await _unitOfWork.StationTriNMRepository.GetAllAsync();
        }

        public async Task<StationTriNm?> GetStationByIdAsync(int id)
        {
            return await _unitOfWork.StationTriNMRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<StationTriNm>> GetActiveStationsAsync()
        {
            return await _unitOfWork.StationTriNMRepository.GetActiveStationsAsync();
        }

        public async Task<StationTriNm?> GetStationWithChargersAsync(int stationTriNMId)
        {
            return await _unitOfWork.StationTriNMRepository.GetStationWithChargersAsync(stationTriNMId);
        }

        public async Task<IEnumerable<StationTriNm>> GetStationsByLocationAsync(string location)
        {
            return await _unitOfWork.StationTriNMRepository.GetStationsByLocationAsync(location);
        }

        public async Task<IEnumerable<StationTriNm>> SearchStationsAsync(string? name, string? location, bool? isActive)
        {
            return await _unitOfWork.StationTriNMRepository.SearchStationsAsync(name, location, isActive);
        }

        public async Task<(IEnumerable<StationTriNm> Items, int TotalCount)> SearchStationsWithPaginationAsync(
            string? name, string? location, bool? isActive, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            return await _unitOfWork.StationTriNMRepository.SearchStationsWithPaginationAsync(
                name, location, isActive, pageNumber, pageSize);
        }

        public async Task<(IEnumerable<StationTriNm> Items, int TotalCount)> GetStationsWithPaginationAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var allStations = await _unitOfWork.StationTriNMRepository.GetAllAsync();
            var totalCount = allStations.Count();
            var items = allStations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, totalCount);
        }

        public async Task<StationTriNm> CreateStationAsync(StationTriNm station)
        {
            ArgumentNullException.ThrowIfNull(station);

            if (string.IsNullOrWhiteSpace(station.StationTriNmcode))
            {
                throw new ArgumentException("Station Code is required.", nameof(station));
            }

            var existingStation = await _unitOfWork.StationTriNMRepository.GetStationByCodeAsync(station.StationTriNmcode);
            if (existingStation != null)
            {
                throw new InvalidOperationException($"Station with code '{station.StationTriNmcode}' already exists.");
            }

            station.CreatedDate = DateTime.UtcNow;
            station.IsActive = true;

            await _unitOfWork.StationTriNMRepository.AddAsync(station);
            await _unitOfWork.SaveChangesAsync();

            return station;
        }

        public async Task<StationTriNm?> UpdateStationAsync(StationTriNm station)
        {
            ArgumentNullException.ThrowIfNull(station);

            var existingStation = await _unitOfWork.StationTriNMRepository.GetByIdAsync(station.StationTriNmid);
            if (existingStation == null)
            {
                return null;
            }

            station.ModifiedDate = DateTime.UtcNow;
            await _unitOfWork.StationTriNMRepository.UpdateAsync(station);
            await _unitOfWork.SaveChangesAsync();

            return await _unitOfWork.StationTriNMRepository.GetByIdAsync(station.StationTriNmid);
        }

        public async Task<bool> DeleteStationAsync(int id, bool hardDelete = false)
        {
            var station = await _unitOfWork.StationTriNMRepository.GetByIdAsync(id);
            if (station == null)
            {
                return false;
            }

            if (hardDelete)
            {
                await _unitOfWork.StationTriNMRepository.DeleteAsync(station);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                station.IsActive = false;
                station.ModifiedDate = DateTime.UtcNow;
                await _unitOfWork.StationTriNMRepository.UpdateAsync(station);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> ActivateStationAsync(int id)
        {
            var station = await _unitOfWork.StationTriNMRepository.GetByIdAsync(id);
            if (station == null)
            {
                return false;
            }

            if (station.IsActive)
            {
                return true;
            }

            station.IsActive = true;
            station.ModifiedDate = DateTime.UtcNow;
            await _unitOfWork.StationTriNMRepository.UpdateAsync(station);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

