using System.ComponentModel.DataAnnotations;

namespace EVCS.TriNM.Services.Object.Requests
{
    public class UpdateUserProfileRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Employee code cannot exceed 50 characters")]
        public string? EmployeeCode { get; set; }
    }
}
