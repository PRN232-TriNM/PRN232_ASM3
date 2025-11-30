using Grpc.Net.Client;
using EVCS.GrpcService.TriNM.Protos;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
Console.WriteLine("=== EVCS gRPC Client - Station Management ===");
Console.WriteLine();

try
{
    var httpHandler = new HttpClientHandler();
    httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    
    using var channel = GrpcChannel.ForAddress("http://localhost:5112", new GrpcChannelOptions
    {
        HttpHandler = httpHandler
    });
    var client = new StationGRPC.StationGRPCClient(channel);

    bool continueRunning = true;

    while (continueRunning)
    {
        ShowMainMenu();

        Console.Write("Chọn chức năng (1-7): ");
        string? choice = Console.ReadLine();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                await CreateStationAsync(client);
                break;
            case "2":
                await ReadStationsAsync(client);
                break;
            case "3":
                await UpdateStationAsync(client);
                break;
            case "4":
                await DeleteStationAsync(client);
                break;
            case "5":
                await ActivateStationAsync(client);
                break;
            case "6":
                await UpdateAvailabilityAsync(client);
                break;
            case "7":
                continueRunning = false;
                Console.WriteLine("Cảm ơn bạn đã sử dụng! Tạm biệt!");
                break;
            default:
                Console.WriteLine("❌ Lựa chọn không hợp lệ! Vui lòng chọn lại.");
                break;
        }

        if (continueRunning)
        {
            Console.WriteLine();
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
catch (Grpc.Core.RpcException ex)
{
    Console.WriteLine("❌ LỖI: Không thể kết nối đến gRPC Service!");
    Console.WriteLine($"   Chi tiết: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("💡 GIẢI PHÁP:");
    Console.WriteLine("   1. Đảm bảo gRPC Service đang chạy");
    Console.WriteLine("   2. Chạy: cd EVCS.GrpcService.TriNM && dotnet run --launch-profile http");
    Console.WriteLine("   3. Kiểm tra service tại: http://localhost:5112");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ LỖI: {ex.Message}");
    Console.WriteLine($"   Stack Trace: {ex.StackTrace}");
}

Console.WriteLine();
Console.WriteLine("Nhấn phím bất kỳ để thoát...");
Console.ReadKey();


static void ShowMainMenu()
{
    Console.WriteLine("╔════════════════════════════════════════╗");
    Console.WriteLine("║     QUẢN LÝ TRẠM SẠC ĐIỆN (STATION)    ║");
    Console.WriteLine("╠════════════════════════════════════════╣");
    Console.WriteLine("║  1. CREATE - Tạo trạm sạc mới         ║");
    Console.WriteLine("║  2. READ   - Xem danh sách trạm       ║");
    Console.WriteLine("║  3. UPDATE - Cập nhật trạm            ║");
    Console.WriteLine("║  4. DELETE - Xóa trạm                 ║");
    Console.WriteLine("║  5. ACTIVATE - Kích hoạt trạm         ║");
    Console.WriteLine("║  6. UPDATE AVAILABILITY - Cập nhật SL ║");
    Console.WriteLine("║  7. THOÁT                              ║");
    Console.WriteLine("╚════════════════════════════════════════╝");
    Console.WriteLine();
}


static string ReadRequiredString(string prompt, int maxLength)
{
    while (true)
    {
        Console.Write($"{prompt} *: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine($"❌ {prompt} là bắt buộc. Vui lòng nhập lại.");
            continue;
        }
        if (input.Length > maxLength)
        {
            Console.WriteLine($"❌ {prompt} không được vượt quá {maxLength} ký tự. Vui lòng nhập lại.");
            continue;
        }
        return input;
    }
}

static string? ReadOptionalString(string prompt, int maxLength)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        if (input.Length > maxLength)
        {
            Console.WriteLine($"❌ {prompt} không được vượt quá {maxLength} ký tự. Vui lòng nhập lại.");
            continue;
        }
        return input;
    }
}

static int ReadRequiredInt(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write($"{prompt} * (từ {min} đến {max}): ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine($"❌ {prompt} là bắt buộc. Vui lòng nhập lại.");
            continue;
        }
        if (!int.TryParse(input, out int value))
        {
            Console.WriteLine($"❌ {prompt} phải là số nguyên. Vui lòng nhập lại.");
            continue;
        }
        if (value < min || value > max)
        {
            Console.WriteLine($"❌ {prompt} phải từ {min} đến {max}. Vui lòng nhập lại.");
            continue;
        }
        return value;
    }
}

static int? ReadOptionalInt(string prompt, int? min = null, int? max = null)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        if (!int.TryParse(input, out int value))
        {
            Console.WriteLine($"❌ {prompt} phải là số nguyên. Vui lòng nhập lại.");
            continue;
        }
        if (min.HasValue && value < min.Value)
        {
            Console.WriteLine($"❌ {prompt} phải lớn hơn hoặc bằng {min.Value}. Vui lòng nhập lại.");
            continue;
        }
        if (max.HasValue && value > max.Value)
        {
            Console.WriteLine($"❌ {prompt} phải nhỏ hơn hoặc bằng {max.Value}. Vui lòng nhập lại.");
            continue;
        }
        return value;
    }
}

static double? ReadOptionalDouble(string prompt, double min, double max)
{
    while (true)
    {
        Console.Write($"{prompt} (từ {min} đến {max}): ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        if (!double.TryParse(input, out double value))
        {
            Console.WriteLine($"❌ {prompt} phải là số thập phân. Vui lòng nhập lại.");
            continue;
        }
        if (value < min || value > max)
        {
            Console.WriteLine($"❌ {prompt} phải từ {min} đến {max}. Vui lòng nhập lại.");
            continue;
        }
        return value;
    }
}

static string? ReadOptionalPhone(string prompt)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        if (input.Length > 20)
        {
            Console.WriteLine($"❌ {prompt} không được vượt quá 20 ký tự. Vui lòng nhập lại.");
            continue;
        }
        if (!Regex.IsMatch(input, @"^[\d\s\-\(\)\+]+$"))
        {
            Console.WriteLine($"❌ {prompt} không đúng định dạng số điện thoại. Vui lòng nhập lại.");
            continue;
        }
        return input;
    }
}

static string? ReadOptionalEmail(string prompt)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        if (input.Length > 150)
        {
            Console.WriteLine($"❌ {prompt} không được vượt quá 150 ký tự. Vui lòng nhập lại.");
            continue;
        }
        if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            Console.WriteLine($"❌ {prompt} không đúng định dạng email. Vui lòng nhập lại.");
            continue;
        }
        return input;
    }
}

static string? ReadOptionalStringWithDefault(string prompt, string? defaultValue, int maxLength)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultValue;
        }
        if (input.Length > maxLength)
        {
            Console.WriteLine($"❌ Không được vượt quá {maxLength} ký tự. Vui lòng nhập lại.");
            continue;
        }
        return input;
    }
}

static int ReadOptionalIntWithDefault(string prompt, int defaultValue, int? min = null, int? max = null)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultValue;
        }
        if (!int.TryParse(input, out int value))
        {
            Console.WriteLine($"❌ Phải là số nguyên. Vui lòng nhập lại.");
            continue;
        }
        if (min.HasValue && value < min.Value)
        {
            Console.WriteLine($"❌ Phải lớn hơn hoặc bằng {min.Value}. Vui lòng nhập lại.");
            continue;
        }
        if (max.HasValue && value > max.Value)
        {
            Console.WriteLine($"❌ Phải nhỏ hơn hoặc bằng {max.Value}. Vui lòng nhập lại.");
            continue;
        }
        return value;
    }
}

static string? ReadOptionalPhoneWithDefault(string prompt, string? defaultValue)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultValue;
        }
        if (input.Length > 20)
        {
            Console.WriteLine($"❌ Không được vượt quá 20 ký tự. Vui lòng nhập lại.");
            continue;
        }
        if (!Regex.IsMatch(input, @"^[\d\s\-\(\)\+]+$"))
        {
            Console.WriteLine($"❌ Không đúng định dạng số điện thoại. Vui lòng nhập lại.");
            continue;
        }
        return input;
    }
}

static string? ReadOptionalEmailWithDefault(string prompt, string? defaultValue)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultValue;
        }
        if (input.Length > 150)
        {
            Console.WriteLine($"❌ Không được vượt quá 150 ký tự. Vui lòng nhập lại.");
            continue;
        }
        if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            Console.WriteLine($"❌ Không đúng định dạng email. Vui lòng nhập lại.");
            continue;
        }
        return input;
    }
}

static double? ReadOptionalDoubleWithDefault(string prompt, double? defaultValue, double min, double max)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultValue;
        }
        if (!double.TryParse(input, out double value))
        {
            Console.WriteLine($"❌ Phải là số thập phân. Vui lòng nhập lại.");
            continue;
        }
        if (value < min || value > max)
        {
            Console.WriteLine($"❌ Phải từ {min} đến {max}. Vui lòng nhập lại.");
            continue;
        }
        return value;
    }
}


static async Task CreateStationAsync(StationGRPC.StationGRPCClient client)
{
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("  TẠO TRẠM SẠC MỚI (CREATE)");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine();

    try
    {
        var request = new CreateStationRequest();

        request.StationCode = ReadRequiredString("Mã trạm (Station Code)", maxLength: 50);
        request.StationName = ReadRequiredString("Tên trạm (Station Name)", maxLength: 100);
        request.Address = ReadRequiredString("Địa chỉ (Address)", maxLength: 200);
        request.City = ReadOptionalString("Thành phố (City)", maxLength: 100) ?? "";
        request.Province = ReadOptionalString("Tỉnh/Thành phố (Province)", maxLength: 100) ?? "";
        
        var latitude = ReadOptionalDouble("Vĩ độ (Latitude)", min: -90, max: 90);
        request.Latitude = latitude ?? 0;
        
        var longitude = ReadOptionalDouble("Kinh độ (Longitude)", min: -180, max: 180);
        request.Longitude = longitude ?? 0;
        
        request.Capacity = ReadRequiredInt("Sức chứa (Capacity)", min: 1, max: 1000);
        
        int? available = ReadOptionalInt("Số lượng hiện có (Current Available)", min: 0, max: request.Capacity);
        request.CurrentAvailable = available ?? request.Capacity;
        
        request.Owner = ReadRequiredString("Chủ sở hữu (Owner)", maxLength: 150);
        request.ContactPhone = ReadOptionalPhone("Số điện thoại liên hệ (Contact Phone)") ?? "";
        request.ContactEmail = ReadOptionalEmail("Email liên hệ (Contact Email)") ?? "";
        request.Description = ReadOptionalString("Mô tả (Description)", maxLength: 500) ?? "";
        request.ImageURL = ReadOptionalString("Image URL", maxLength: 500) ?? "";

        Console.WriteLine();
        Console.WriteLine("Đang tạo trạm sạc...");

        var response = await client.CreateAsyncAsync(request);

        Console.WriteLine();
        Console.WriteLine($"✅ Tạo trạm sạc thành công! ID: {response.Result}");
    }
    catch (Grpc.Core.RpcException ex)
    {
        Console.WriteLine($"❌ Lỗi: {ex.Status.Detail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi không mong đợi: {ex.Message}");
    }
}


static async Task ReadStationsAsync(StationGRPC.StationGRPCClient client)
{
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("  XEM DANH SÁCH TRẠM (READ)");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("1. Xem tất cả trạm");
    Console.WriteLine("2. Xem trạm có sẵn");
    Console.WriteLine("3. Tìm kiếm trạm");
    Console.WriteLine("4. Xem chi tiết trạm theo ID");
    Console.WriteLine();
    Console.Write("Chọn (1-4): ");
    string? choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        StationList stationsList;

        switch (choice)
        {
            case "1":
                Console.WriteLine("Đang tải danh sách tất cả trạm...");
                stationsList = await client.GetAllAsyncAsync(new EmptyRequest());
                break;
            case "2":
                Console.WriteLine("Đang tải danh sách trạm có sẵn...");
                stationsList = await client.GetActiveStationsAsyncAsync(new EmptyRequest());
                break;
            case "3":
                Console.Write("Nhập từ khóa tìm kiếm: ");
                string? searchTerm = Console.ReadLine();
                Console.WriteLine("Đang tìm kiếm...");
                stationsList = await client.SearchStationsAsyncAsync(new SearchRequest { SearchTerm = searchTerm ?? "" });
                break;
            case "4":
                Console.Write("Nhập ID trạm: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var station = await client.GetByIdAsyncAsync(new StationRequest { StationId = id });
                    if (station != null && station.StationId > 0)
                    {
                        DisplayStation(station);
                    }
                    else
                    {
                        Console.WriteLine("❌ Không tìm thấy trạm với ID này.");
                    }
                }
                else
                {
                    Console.WriteLine("❌ ID không hợp lệ.");
                }
                return;
            default:
                Console.WriteLine("❌ Lựa chọn không hợp lệ.");
                return;
        }

        var stations = stationsList?.Items?.ToList() ?? new List<Station>();

        if (stations != null && stations.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"📋 Tìm thấy {stations.Count} trạm:");
            Console.WriteLine();
            Console.WriteLine(new string('═', 120));
            Console.WriteLine($"{"ID",-5} {"Mã",-12} {"Tên",-25} {"Địa chỉ",-30} {"SL",-5} {"TT",-5}");
            Console.WriteLine(new string('═', 120));
            foreach (var station in stations)
            {
                string status = station.IsActive ? "✓" : "✗";
                Console.WriteLine($"{station.StationId,-5} {station.StationCode,-12} {station.StationName,-25} " +
                    $"{station.Address,-30} {station.CurrentAvailable}/{station.Capacity,-5} {status,-5}");
            }
            Console.WriteLine(new string('═', 120));
            Console.WriteLine();
            Console.Write("Nhập ID để xem chi tiết (hoặc Enter để quay lại): ");
            string? detailId = Console.ReadLine();
            if (int.TryParse(detailId, out int detailIdInt))
            {
                var detailStation = await client.GetByIdAsyncAsync(new StationRequest { StationId = detailIdInt });
                if (detailStation != null && detailStation.StationId > 0)
                {
                    Console.WriteLine();
                    DisplayStation(detailStation);
                }
                else
                {
                    Console.WriteLine("❌ Không tìm thấy trạm với ID này.");
                }
            }
        }
        else
        {
            Console.WriteLine("❌ Không tìm thấy trạm nào.");
        }
    }
    catch (Grpc.Core.RpcException ex)
    {
        Console.WriteLine($"❌ Lỗi: {ex.Status.Detail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi không mong đợi: {ex.Message}");
    }
}


static async Task UpdateStationAsync(StationGRPC.StationGRPCClient client)
{
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("  CẬP NHẬT TRẠM (UPDATE)");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine();

    try
    {
        Console.Write("Nhập ID trạm cần cập nhật: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID không hợp lệ.");
            return;
        }

        Console.WriteLine("Đang tải thông tin trạm...");
        var existingStation = await client.GetByIdAsyncAsync(new StationRequest { StationId = id });
        if (existingStation == null || existingStation.StationId == 0)
        {
            Console.WriteLine($"❌ Không tìm thấy trạm với ID {id}.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Thông tin hiện tại:");
        DisplayStation(existingStation);
        Console.WriteLine();
        Console.WriteLine("Nhập thông tin mới (Enter để giữ nguyên):");
        Console.WriteLine();

        var station = new Station
        {
            StationId = existingStation.StationId,
            StationCode = existingStation.StationCode,
            StationName = existingStation.StationName,
            Address = existingStation.Address,
            City = existingStation.City,
            Province = existingStation.Province,
            Latitude = existingStation.Latitude,
            Longitude = existingStation.Longitude,
            Capacity = existingStation.Capacity,
            CurrentAvailable = existingStation.CurrentAvailable,
            Owner = existingStation.Owner,
            ContactPhone = existingStation.ContactPhone,
            ContactEmail = existingStation.ContactEmail,
            Description = existingStation.Description,
            IsActive = existingStation.IsActive,
            ImageURL = existingStation.ImageURL,
            CreatedDate = existingStation.CreatedDate,
            ModifiedDate = existingStation.ModifiedDate
        };

        station.StationCode = ReadOptionalStringWithDefault($"Mã trạm (Station Code) [{station.StationCode}]", station.StationCode, maxLength: 50) ?? station.StationCode;
        station.StationName = ReadOptionalStringWithDefault($"Tên trạm (Station Name) [{station.StationName}]", station.StationName, maxLength: 100) ?? station.StationName;
        station.Address = ReadOptionalStringWithDefault($"Địa chỉ (Address) [{station.Address}]", station.Address, maxLength: 200) ?? station.Address;
        station.City = ReadOptionalStringWithDefault($"Thành phố (City) [{station.City ?? "N/A"}]", station.City, maxLength: 100) ?? "";
        station.Province = ReadOptionalStringWithDefault($"Tỉnh/Thành phố (Province) [{station.Province ?? "N/A"}]", station.Province, maxLength: 100) ?? "";
        
        string? latPrompt = station.Latitude != 0 ? $"Vĩ độ (Latitude) [{station.Latitude}]" : "Vĩ độ (Latitude) [N/A]";
        double? newLat = ReadOptionalDoubleWithDefault(latPrompt, station.Latitude != 0 ? station.Latitude : null, min: -90, max: 90);
        station.Latitude = newLat ?? 0;
        
        string? lngPrompt = station.Longitude != 0 ? $"Kinh độ (Longitude) [{station.Longitude}]" : "Kinh độ (Longitude) [N/A]";
        double? newLng = ReadOptionalDoubleWithDefault(lngPrompt, station.Longitude != 0 ? station.Longitude : null, min: -180, max: 180);
        station.Longitude = newLng ?? 0;
        
        station.Capacity = ReadOptionalIntWithDefault($"Sức chứa (Capacity) [{station.Capacity}]", station.Capacity, min: 1, max: 1000);
        station.CurrentAvailable = ReadOptionalIntWithDefault($"Số lượng hiện có (Current Available) [{station.CurrentAvailable}]", station.CurrentAvailable, min: 0, max: station.Capacity);
        station.Owner = ReadOptionalStringWithDefault($"Chủ sở hữu (Owner) [{station.Owner}]", station.Owner, maxLength: 150) ?? station.Owner;
        station.ContactPhone = ReadOptionalPhoneWithDefault($"Số điện thoại (Contact Phone) [{station.ContactPhone ?? "N/A"}]", station.ContactPhone) ?? "";
        station.ContactEmail = ReadOptionalEmailWithDefault($"Email (Contact Email) [{station.ContactEmail ?? "N/A"}]", station.ContactEmail) ?? "";
        station.Description = ReadOptionalStringWithDefault($"Mô tả (Description) [{station.Description ?? "N/A"}]", station.Description, maxLength: 500) ?? "";
        station.ImageURL = ReadOptionalStringWithDefault($"Image URL [{station.ImageURL ?? "N/A"}]", station.ImageURL, maxLength: 500) ?? "";

        Console.WriteLine();
        Console.WriteLine("Đang cập nhật...");

        var response = await client.UpdateAsyncAsync(station);

        Console.WriteLine();
        Console.WriteLine($"✅ Cập nhật trạm thành công! ID: {response.Result}");
    }
    catch (Grpc.Core.RpcException ex)
    {
        Console.WriteLine($"❌ Lỗi: {ex.Status.Detail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi không mong đợi: {ex.Message}");
    }
}


static async Task DeleteStationAsync(StationGRPC.StationGRPCClient client)
{
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("  XÓA TRẠM (DELETE)");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine();

    try
    {
        Console.Write("Nhập ID trạm cần xóa: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID không hợp lệ.");
            return;
        }

        Console.WriteLine("Đang tải thông tin trạm...");
        var station = await client.GetByIdAsyncAsync(new StationRequest { StationId = id });
        if (station == null || station.StationId == 0)
        {
            Console.WriteLine($"❌ Không tìm thấy trạm với ID {id}.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Thông tin trạm sẽ bị xóa:");
        DisplayStation(station);
        Console.WriteLine();
        Console.Write("⚠️  Bạn có chắc chắn muốn xóa? (y/n): ");
        string? confirm = Console.ReadLine();

        if (confirm?.ToLower() == "y" || confirm?.ToLower() == "yes")
        {
            Console.WriteLine();
            Console.WriteLine("Đang xóa...");

            var response = await client.DeleteAsyncAsync(new StationRequest { StationId = id });

            if (response.Result)
            {
                Console.WriteLine();
                Console.WriteLine("✅ Xóa trạm thành công!");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("❌ Xóa trạm thất bại.");
            }
        }
        else
        {
            Console.WriteLine("Đã hủy thao tác xóa.");
        }
    }
    catch (Grpc.Core.RpcException ex)
    {
        Console.WriteLine($"❌ Lỗi: {ex.Status.Detail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi không mong đợi: {ex.Message}");
    }
}


static async Task ActivateStationAsync(StationGRPC.StationGRPCClient client)
{
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("  KÍCH HOẠT TRẠM (ACTIVATE)");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine();

    try
    {
        Console.Write("Nhập ID trạm cần kích hoạt: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID không hợp lệ.");
            return;
        }

        Console.WriteLine("Đang kích hoạt...");

        var response = await client.ActivateAsyncAsync(new StationRequest { StationId = id });

        if (response.Result)
        {
            Console.WriteLine();
            Console.WriteLine("✅ Kích hoạt trạm thành công!");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("❌ Kích hoạt trạm thất bại. Kiểm tra lại ID.");
        }
    }
    catch (Grpc.Core.RpcException ex)
    {
        Console.WriteLine($"❌ Lỗi: {ex.Status.Detail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi không mong đợi: {ex.Message}");
    }
}


static async Task UpdateAvailabilityAsync(StationGRPC.StationGRPCClient client)
{
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("  CẬP NHẬT SỐ LƯỢNG (UPDATE AVAILABILITY)");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine();

    try
    {
        Console.Write("Nhập ID trạm: ");
        if (!int.TryParse(Console.ReadLine(), out int stationId))
        {
            Console.WriteLine("❌ ID không hợp lệ.");
            return;
        }

        Console.Write("Nhập số lượng mới (Current Available): ");
        if (!int.TryParse(Console.ReadLine(), out int newAvailable))
        {
            Console.WriteLine("❌ Số lượng không hợp lệ.");
            return;
        }

        Console.WriteLine("Đang cập nhật...");

        var response = await client.UpdateAvailabilityAsyncAsync(new UpdateAvailabilityRequest 
        { 
            StationId = stationId, 
            CurrentAvailable = newAvailable 
        });

        if (response.Result)
        {
            Console.WriteLine();
            Console.WriteLine("✅ Cập nhật số lượng thành công!");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("❌ Cập nhật số lượng thất bại.");
        }
    }
    catch (Grpc.Core.RpcException ex)
    {
        Console.WriteLine($"❌ Lỗi: {ex.Status.Detail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi không mong đợi: {ex.Message}");
    }
}


static void DisplayStation(Station station)
{
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("  CHI TIẾT TRẠM SẠC");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine($"ID:              {station.StationId}");
    Console.WriteLine($"Mã trạm:         {station.StationCode}");
    Console.WriteLine($"Tên trạm:        {station.StationName}");
    Console.WriteLine($"Địa chỉ:         {station.Address}");
    Console.WriteLine($"Thành phố:       {station.City ?? "N/A"}");
    Console.WriteLine($"Tỉnh/Thành phố:  {station.Province ?? "N/A"}");
    if (station.Latitude != 0 && station.Longitude != 0)
    {
        Console.WriteLine($"Tọa độ:          ({station.Latitude}, {station.Longitude})");
    }
    Console.WriteLine($"Sức chứa:        {station.Capacity}");
    Console.WriteLine($"Số lượng hiện có: {station.CurrentAvailable}");
    Console.WriteLine($"Chủ sở hữu:      {station.Owner}");
    Console.WriteLine($"Số điện thoại:   {station.ContactPhone ?? "N/A"}");
    Console.WriteLine($"Email:           {station.ContactEmail ?? "N/A"}");
    Console.WriteLine($"Mô tả:           {station.Description ?? "N/A"}");
    Console.WriteLine($"Image URL:       {station.ImageURL ?? "N/A"}");
    Console.WriteLine($"Trạng thái:      {(station.IsActive ? "Đang hoạt động" : "Không hoạt động")}");
    if (!string.IsNullOrEmpty(station.CreatedDate))
    {
        Console.WriteLine($"Ngày tạo:        {station.CreatedDate}");
    }
    if (!string.IsNullOrEmpty(station.ModifiedDate))
    {
        Console.WriteLine($"Ngày sửa:        {station.ModifiedDate}");
    }
    Console.WriteLine("═══════════════════════════════════════");
}
