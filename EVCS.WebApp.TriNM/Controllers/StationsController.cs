using EVCS.GrpcService.TriNM.Protos;
using EVCS.WebApp.TriNM.Models;
using Grpc.Core;
using GrpcStatusCode = Grpc.Core.StatusCode;
using Microsoft.AspNetCore.Mvc;

namespace EVCS.WebApp.TriNM.Controllers
{
    public class StationsController : Controller
    {
        private readonly StationGRPC.StationGRPCClient _client;
        private readonly ILogger<StationsController> _logger;

        public StationsController(StationGRPC.StationGRPCClient client, ILogger<StationsController> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? name, string? location, bool? isActive, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var searchRequest = new SearchRequest
                {
                    Name = name ?? "",
                    Location = location ?? "",
                    HasIsActiveFilter = isActive.HasValue,
                    IsActive = isActive ?? false,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var resp = await _client.SearchStationsAsyncAsync(searchRequest);

                if (resp == null)
                {
                    _logger.LogWarning("gRPC SearchStationsAsync returned null response");
                    return View(new StationSearchViewModel());
                }

                var viewModel = new StationSearchViewModel
                {
                    Name = name,
                    Location = location,
                    IsActive = isActive,
                    PageNumber = resp.PageNumber,
                    PageSize = resp.PageSize,
                    TotalCount = resp.TotalCount,
                    TotalPages = resp.TotalPages,
                    Items = resp.Items.Select(s => new StationViewModel
                    {
                        StationId = s.StationId,
                        StationCode = s.StationCode,
                        StationName = s.StationName,
                        Address = s.Address,
                        City = s.City,
                        Province = s.Province,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        Capacity = s.Capacity,
                        CurrentAvailable = s.CurrentAvailable,
                        Owner = s.Owner,
                        ContactPhone = s.ContactPhone,
                        ContactEmail = s.ContactEmail,
                        Description = s.Description,
                        IsActive = s.IsActive,
                        ImageURL = s.ImageURL,
                        CreatedDate = s.CreatedDate,
                        ModifiedDate = s.ModifiedDate
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.Unavailable)
            {
                _logger.LogError(ex, "gRPC service is not available");
                ViewBag.ErrorMessage = "Cannot connect to gRPC service. Please ensure the service is running.";
                return View(new StationSearchViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stations");
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View(new StationSearchViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var resp = await _client.GetByIdAsyncAsync(new StationRequest { StationId = id });

                if (resp == null || resp.StationId == 0)
                {
                    return NotFound();
                }

                var vm = new StationViewModel
                {
                    StationId = resp.StationId,
                    StationCode = resp.StationCode,
                    StationName = resp.StationName,
                    Address = resp.Address,
                    City = resp.City,
                    Province = resp.Province,
                    Latitude = resp.Latitude,
                    Longitude = resp.Longitude,
                    Capacity = resp.Capacity,
                    CurrentAvailable = resp.CurrentAvailable,
                    Owner = resp.Owner,
                    ContactPhone = resp.ContactPhone,
                    ContactEmail = resp.ContactEmail,
                    Description = resp.Description,
                    IsActive = resp.IsActive,
                    ImageURL = resp.ImageURL,
                    CreatedDate = resp.CreatedDate,
                    ModifiedDate = resp.ModifiedDate
                };

                return View(vm);
            }
            catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.NotFound)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station details");
                return Redirect(Url.Action("Index", "Stations") ?? "/Stations");
            }
        }

        public IActionResult Create()
        {
            if (Request.QueryString.HasValue)
            {
                return RedirectToAction(nameof(Create));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StationViewModel model)
        {
            try
            {
                var request = new CreateStationRequest
                {
                    StationCode = model.StationCode,
                    StationName = model.StationName,
                    Address = model.Address,
                    City = model.City ?? "",
                    Province = model.Province ?? "",
                    Latitude = model.Latitude ?? 0,
                    Longitude = model.Longitude ?? 0,
                    Capacity = model.Capacity,
                    CurrentAvailable = model.CurrentAvailable,
                    Owner = model.Owner,
                    ContactPhone = model.ContactPhone ?? "",
                    ContactEmail = model.ContactEmail ?? "",
                    Description = model.Description ?? "",
                    ImageURL = model.ImageURL ?? ""
                };

                var result = await _client.CreateAsyncAsync(request);
                
                if (result != null && result.Result > 0)
                {
                    TempData["SuccessMessage"] = $"Station '{model.StationName}' has been created successfully!";
                    return Json(new { success = true, redirectUrl = "/Stations" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to create station. Please try again." });
                }
            }
            catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.AlreadyExists)
            {
                return Json(new { success = false, message = ex.Status.Detail ?? "Station Code already exists. Please use a different code." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating station");
                return Json(new { success = false, message = "An error occurred while creating the station." });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (Request.QueryString.HasValue)
            {
                return RedirectToAction(nameof(Edit), new { id });
            }
            try
            {
                var resp = await _client.GetByIdAsyncAsync(new StationRequest { StationId = id });

                if (resp == null || resp.StationId == 0)
                {
                    return NotFound();
                }

                var vm = new StationViewModel
                {
                    StationId = resp.StationId,
                    StationCode = resp.StationCode,
                    StationName = resp.StationName,
                    Address = resp.Address,
                    City = resp.City,
                    Province = resp.Province,
                    Latitude = resp.Latitude,
                    Longitude = resp.Longitude,
                    Capacity = resp.Capacity,
                    CurrentAvailable = resp.CurrentAvailable,
                    Owner = resp.Owner,
                    ContactPhone = resp.ContactPhone,
                    ContactEmail = resp.ContactEmail,
                    Description = resp.Description,
                    IsActive = resp.IsActive,
                    ImageURL = resp.ImageURL,
                    CreatedDate = resp.CreatedDate,
                    ModifiedDate = resp.ModifiedDate
                };

                return View(vm);
            }
            catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.NotFound)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station for edit");
                return Redirect(Url.Action("Index", "Stations") ?? "/Stations");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StationViewModel model)
        {
            try
            {
                var station = new Station
                {
                    StationId = model.StationId,
                    StationCode = model.StationCode,
                    StationName = model.StationName,
                    Address = model.Address,
                    City = model.City ?? "",
                    Province = model.Province ?? "",
                    Latitude = model.Latitude ?? 0,
                    Longitude = model.Longitude ?? 0,
                    Capacity = model.Capacity,
                    CurrentAvailable = model.CurrentAvailable,
                    Owner = model.Owner,
                    ContactPhone = model.ContactPhone ?? "",
                    ContactEmail = model.ContactEmail ?? "",
                    Description = model.Description ?? "",
                    IsActive = model.IsActive,
                    ImageURL = model.ImageURL ?? ""
                };

                var result = await _client.UpdateAsyncAsync(station);
                
                if (result.Result > 0)
                {
                    TempData["SuccessMessage"] = $"Station '{model.StationName}' has been updated successfully!";
                    return Json(new { success = true, redirectUrl = "/Stations" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update station. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating station");
                return Json(new { success = false, message = "An error occurred while updating the station." });
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resp = await _client.GetByIdAsyncAsync(new StationRequest { StationId = id });

                if (resp == null || resp.StationId == 0)
                {
                    return Redirect(Url.Action("Index", "Stations") ?? "/Stations");
                }

                var vm = new StationViewModel
                {
                    StationId = resp.StationId,
                    StationCode = resp.StationCode,
                    StationName = resp.StationName,
                    Address = resp.Address,
                    City = resp.City,
                    Province = resp.Province,
                    Latitude = resp.Latitude,
                    Longitude = resp.Longitude,
                    Capacity = resp.Capacity,
                    CurrentAvailable = resp.CurrentAvailable,
                    Owner = resp.Owner,
                    ContactPhone = resp.ContactPhone,
                    ContactEmail = resp.ContactEmail,
                    Description = resp.Description,
                    IsActive = resp.IsActive,
                    ImageURL = resp.ImageURL,
                    CreatedDate = resp.CreatedDate,
                    ModifiedDate = resp.ModifiedDate
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station for delete");
                return Redirect(Url.Action("Index", "Stations") ?? "/Stations");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _client.DeleteAsyncAsync(new StationRequest { StationId = id });
                TempData["SuccessMessage"] = "Station has been deleted successfully!";
                return Json(new { success = true, redirectUrl = "/Stations" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting station");
                return Json(new { success = false, message = "An error occurred while deleting the station." });
            }
        }
    }
}

