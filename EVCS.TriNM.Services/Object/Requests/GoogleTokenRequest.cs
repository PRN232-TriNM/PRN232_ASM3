using System.ComponentModel.DataAnnotations;

namespace EVCS.TriNM.Services.Object.Requests
{
    public class GoogleTokenRequest
    {
        [Required(ErrorMessage = "Google credential is required")]
        public string Credential { get; set; } = string.Empty;
    }
}
