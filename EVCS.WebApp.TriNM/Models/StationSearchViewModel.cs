using System.ComponentModel;

namespace EVCS.WebApp.TriNM.Models
{
    public class StationSearchViewModel
    {
        [DisplayName("Name")]
        public string? Name { get; set; }

        [DisplayName("Location")]
        public string? Location { get; set; }

        [DisplayName("Status")]
        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<StationViewModel> Items { get; set; } = new List<StationViewModel>();
    }
}

