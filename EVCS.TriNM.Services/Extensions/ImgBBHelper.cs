using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace PRN222.SU25.SE1709.G5.GenderCareHMS.Services.TriNM.Extensions
{
    public class ImgBBHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<ImgBBHelper> _logger;
        private const string UploadUrl = "https://api.imgbb.com/1/upload";

        public ImgBBHelper(HttpClient httpClient, IConfiguration configuration, ILogger<ImgBBHelper> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = configuration["ImgBB:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "ImgBB API key is not configured.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required.", nameof(imageFile));

            try
            {
                using var stream = imageFile.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var base64Image = Convert.ToBase64String(memoryStream.ToArray());

                return await UploadImageAsync(base64Image);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting IFormFile to base64");
                throw new InvalidOperationException("Failed to process image file.", ex);
            }
        }

        public async Task<string> UploadImageAsync(string base64Image, int expirationSeconds = 600)
        {
            if (string.IsNullOrEmpty(base64Image))
                throw new ArgumentException("Base64 image data is required.", nameof(base64Image));

            if (expirationSeconds < 60 || expirationSeconds > 15552000)
                throw new ArgumentException("Expiration must be between 60 and 15552000 seconds.", nameof(expirationSeconds));

            try
            {
                var content = new MultipartFormDataContent
                {
                    { new StringContent(_apiKey), "key" },
                    { new StringContent(base64Image), "image" },
                    { new StringContent(expirationSeconds.ToString()), "expiration" }
                };

                var response = await _httpClient.PostAsync(UploadUrl, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var url = doc.RootElement.GetProperty("data").GetProperty("url").GetString();

                if (string.IsNullOrEmpty(url))
                    throw new InvalidOperationException("ImgBB API did not return a valid URL.");

                _logger.LogInformation("Uploaded image to ImgBB: {Url}", url);
                return url;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error uploading image to ImgBB: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to upload image to ImgBB due to a network error.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing ImgBB response: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to parse ImgBB API response.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error uploading image to ImgBB");
                throw new InvalidOperationException("Unexpected error occurred while uploading image to ImgBB.", ex);
            }
        }
    }
}