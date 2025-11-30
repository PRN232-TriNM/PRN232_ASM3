using EVCS.GrpcService.TriNM.Protos;
using EVCS.GrpcService.TriNM.Hubs;
using EVCS.TriNM.Services.Implements;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.Json.Serialization;
using StationModel = EVCS.TriNM.Repositories.Models.StationTriNm;

namespace EVCS.GrpcService.TriNM
{
    public class StationGRPCService : StationGRPC.StationGRPCBase
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IHubContext<StationHub> _hubContext;

        public StationGRPCService(IServiceProviders serviceProviders, IHubContext<StationHub> hubContext)
        {
            _serviceProviders = serviceProviders;
            _hubContext = hubContext;
        }

        public override async Task<StationList> GetAllAsync(EmptyRequest request, ServerCallContext context)
        {
            var result = new StationList();
            try
            {
                var stations = await _serviceProviders.StationService.GetAllStationsAsync();
                foreach (var station in stations)
                {
                    result.Items.Add(MapToProto(station));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public override async Task<Protos.Station> GetByIdAsync(StationRequest request, ServerCallContext context)
        {
            var result = new Protos.Station();
            try
            {
                var station = await _serviceProviders.StationService.GetStationByIdAsync(request.StationId);
                if (station == null)
                {
                    return result;
                }
                return MapToProto(station);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override async Task<StationList> GetActiveStationsAsync(EmptyRequest request, ServerCallContext context)
        {
            var result = new StationList();
            try
            {
                var stations = await _serviceProviders.StationService.GetActiveStationsAsync();
                foreach (var station in stations)
                {
                    result.Items.Add(MapToProto(station));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public override async Task<PaginatedStationList> SearchStationsAsync(SearchRequest request, ServerCallContext context)
        {
            var result = new PaginatedStationList();
            try
            {
                int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
                int pageSize = request.PageSize > 0 ? request.PageSize : 10;

                bool? isActiveFilter = request.HasIsActiveFilter ? request.IsActive : null;
                var (stations, totalCount) = await _serviceProviders.StationService.SearchStationsWithPaginationAsync(
                    string.IsNullOrWhiteSpace(request.Name) ? null : request.Name,
                    string.IsNullOrWhiteSpace(request.Location) ? null : request.Location,
                    isActiveFilter,
                    pageNumber,
                    pageSize);

                foreach (var station in stations)
                {
                    result.Items.Add(MapToProto(station));
                }

                result.TotalCount = totalCount;
                result.PageNumber = pageNumber;
                result.PageSize = pageSize;
                result.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public override async Task<PaginatedStationList> GetStationsWithPaginationAsync(PaginationRequest request, ServerCallContext context)
        {
            var result = new PaginatedStationList();
            try
            {
                int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
                int pageSize = request.PageSize > 0 ? request.PageSize : 10;

                var (stations, totalCount) = await _serviceProviders.StationService.GetStationsWithPaginationAsync(pageNumber, pageSize);

                foreach (var station in stations)
                {
                    result.Items.Add(MapToProto(station));
                }

                result.TotalCount = totalCount;
                result.PageNumber = pageNumber;
                result.PageSize = pageSize;
                result.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public override async Task<MutationRelay> CreateAsync(CreateStationRequest createStationRequest, ServerCallContext context)
        {
            try 
            {
                var item = new StationModel
                {
                    StationTriNmcode = createStationRequest.StationCode,
                    StationTriNmname = createStationRequest.StationName,
                    Address = createStationRequest.Address,
                    City = createStationRequest.City,
                    Province = createStationRequest.Province,
                    Latitude = createStationRequest.Latitude != 0 ? (decimal?)createStationRequest.Latitude : null,
                    Longitude = createStationRequest.Longitude != 0 ? (decimal?)createStationRequest.Longitude : null,
                    Capacity = createStationRequest.Capacity,
                    CurrentAvailable = createStationRequest.CurrentAvailable,
                    Owner = createStationRequest.Owner,
                    ContactPhone = createStationRequest.ContactPhone,
                    ContactEmail = createStationRequest.ContactEmail,
                    Description = createStationRequest.Description,
                    ImageUrl = createStationRequest.ImageURL
                };

                Console.WriteLine($"StationCode={item.StationTriNmcode}, StationName={item.StationTriNmname}");
                var createdStation = await _serviceProviders.StationService.CreateStationAsync(item);

                if (createdStation != null)
                {
                    var protoStation = MapToProto(createdStation);
                    var stationJson = JsonSerializer.Serialize(protoStation, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    });
                    await _hubContext.Clients.All.SendAsync("StationCreated", stationJson);
                }

                return new MutationRelay { Result = createdStation?.StationTriNmid ?? 0 };
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[CREATE ERROR] {ex.Message}");
                throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[CREATE ERROR] {ex.Message}");
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CREATE ERROR] {ex.ToString()}");
                throw new RpcException(new Status(StatusCode.Internal, $"An error occurred while creating the station: {ex.Message}"));
            }
        }

        public override async Task<MutationRelay> UpdateAsync(Protos.Station station, ServerCallContext context)
        {
            try
            {
                var existingStation = await _serviceProviders.StationService.GetStationByIdAsync(station.StationId);
                if (existingStation == null)
                {
                    throw new ArgumentException($"Station with ID {station.StationId} not found");
                }

                existingStation.StationTriNmcode = station.StationCode;
                existingStation.StationTriNmname = station.StationName;
                existingStation.Address = station.Address;
                existingStation.City = station.City;
                existingStation.Province = station.Province;
                existingStation.Latitude = station.Latitude != 0 ? (decimal?)station.Latitude : null;
                existingStation.Longitude = station.Longitude != 0 ? (decimal?)station.Longitude : null;
                existingStation.Capacity = station.Capacity;
                existingStation.CurrentAvailable = station.CurrentAvailable;
                existingStation.Owner = station.Owner;
                existingStation.ContactPhone = station.ContactPhone;
                existingStation.ContactEmail = station.ContactEmail;
                existingStation.Description = station.Description;
                existingStation.IsActive = station.IsActive;
                existingStation.ImageUrl = station.ImageURL;

                Console.WriteLine($"[UPDATE] ID={existingStation.StationTriNmid}, StationCode={existingStation.StationTriNmcode}, StationName={existingStation.StationTriNmname}");

                var updatedStation = await _serviceProviders.StationService.UpdateStationAsync(existingStation);

                if (updatedStation != null)
                {
                    var protoStation = MapToProto(updatedStation);
                    var stationJson = JsonSerializer.Serialize(protoStation, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    });
                    await _hubContext.Clients.All.SendAsync("StationUpdated", stationJson);
                    await _hubContext.Clients.Group($"station_{updatedStation.StationTriNmid}").SendAsync("StationUpdated", stationJson);
                }

                return new MutationRelay { Result = updatedStation?.StationTriNmid ?? 0 };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private class DateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var dateString = reader.GetString();
                    if (DateTime.TryParse(dateString, out var date))
                    {
                        return date;
                    }
                }
                return DateTime.MinValue;
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }
        }

        private class NullableDateTimeConverter : JsonConverter<DateTime?>
        {
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }
                if (reader.TokenType == JsonTokenType.String)
                {
                    var dateString = reader.GetString();
                    if (string.IsNullOrEmpty(dateString))
                    {
                        return null;
                    }
                    if (DateTime.TryParse(dateString, out var date))
                    {
                        return date;
                    }
                }
                return null;
            }

            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                {
                    writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                }
                else
                {
                    writer.WriteNullValue();
                }
            }
        }

        public override async Task<Mutation> DeleteAsync(StationRequest request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine($"[DELETE] ID={request.StationId}");

                var result = await _serviceProviders.StationService.DeleteStationAsync(request.StationId, hardDelete: true);

                if (result)
                {
                    await _hubContext.Clients.All.SendAsync("StationDeleted", request.StationId);
                    await _hubContext.Clients.Group($"station_{request.StationId}").SendAsync("StationDeleted", request.StationId);
                }

                return new Mutation { Result = result };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public override async Task<Mutation> ActivateAsync(StationRequest request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine($"[ACTIVATE] ID={request.StationId}");

                var result = await _serviceProviders.StationService.ActivateStationAsync(request.StationId);

                if (result)
                {
                    var station = await _serviceProviders.StationService.GetStationByIdAsync(request.StationId);
                    if (station != null)
                    {
                        var protoStation = MapToProto(station);
                        var stationJson = JsonSerializer.Serialize(protoStation, new JsonSerializerOptions 
                        { 
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                        });
                        await _hubContext.Clients.All.SendAsync("StationActivated", stationJson);
                        await _hubContext.Clients.Group($"station_{request.StationId}").SendAsync("StationActivated", stationJson);
                    }
                }

                return new Mutation { Result = result };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public override async Task<Mutation> UpdateAvailabilityAsync(UpdateAvailabilityRequest request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine($"[UPDATE AVAILABILITY] StationId={request.StationId}, CurrentAvailable={request.CurrentAvailable}");

                var station = await _serviceProviders.StationService.GetStationByIdAsync(request.StationId);
                if (station == null)
                {
                    return new Mutation { Result = false };
                }

                station.CurrentAvailable = request.CurrentAvailable;
                var updatedStation = await _serviceProviders.StationService.UpdateStationAsync(station);

                if (updatedStation != null)
                {
                    var availabilityData = JsonSerializer.Serialize(new
                    {
                        StationId = updatedStation.StationTriNmid,
                        CurrentAvailable = updatedStation.CurrentAvailable
                    }, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    });
                    await _hubContext.Clients.All.SendAsync("StationAvailabilityUpdated", availabilityData);
                    await _hubContext.Clients.Group($"station_{updatedStation.StationTriNmid}").SendAsync("StationAvailabilityUpdated", availabilityData);
                }

                return new Mutation { Result = updatedStation != null };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private Protos.Station MapToProto(StationModel station)
        {
            if (station == null)
            {
                return new Protos.Station();
            }

            return new Protos.Station
            {
                StationId = station.StationTriNmid,
                StationCode = station.StationTriNmcode ?? "",
                StationName = station.StationTriNmname ?? "",
                Address = station.Address ?? "",
                City = station.City ?? "",
                Province = station.Province ?? "",
                Latitude = station.Latitude.HasValue ? (double)station.Latitude.Value : 0,
                Longitude = station.Longitude.HasValue ? (double)station.Longitude.Value : 0,
                Capacity = station.Capacity,
                CurrentAvailable = station.CurrentAvailable,
                Owner = station.Owner ?? "",
                ContactPhone = station.ContactPhone ?? "",
                ContactEmail = station.ContactEmail ?? "",
                Description = station.Description ?? "",
                IsActive = station.IsActive,
                ImageURL = station.ImageUrl ?? "",
                CreatedDate = station.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                ModifiedDate = station.ModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? ""
            };
        }

    }
}

