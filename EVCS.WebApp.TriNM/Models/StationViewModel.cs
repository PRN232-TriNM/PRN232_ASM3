using System.ComponentModel.DataAnnotations;

namespace EVCS.WebApp.TriNM.Models
{
    public class StationViewModel
    {
        public int StationId { get; set; }

        [Required(ErrorMessage = "Station Code is required")]
        [MaxLength(50, ErrorMessage = "Station Code cannot exceed 50 characters")]
        public string StationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Station Name is required")]
        [MaxLength(100, ErrorMessage = "Station Name cannot exceed 100 characters")]
        public string StationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [MaxLength(100, ErrorMessage = "Province cannot exceed 100 characters")]
        public string? Province { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }

        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int Capacity { get; set; }

        [Range(0, 1000, ErrorMessage = "Current Available must be between 0 and 1000")]
        public int CurrentAvailable { get; set; }

        [Required(ErrorMessage = "Owner is required")]
        [MaxLength(150, ErrorMessage = "Owner cannot exceed 150 characters")]
        public string Owner { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20, ErrorMessage = "Contact Phone cannot exceed 20 characters")]
        public string? ContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(150, ErrorMessage = "Contact Email cannot exceed 150 characters")]
        public string? ContactEmail { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageURL { get; set; }

        public string? CreatedDate { get; set; }

        public string? ModifiedDate { get; set; }
    }
}

