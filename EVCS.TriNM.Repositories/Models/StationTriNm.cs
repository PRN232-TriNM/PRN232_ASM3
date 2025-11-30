#nullable disable
using System;
using System.Collections.Generic;

namespace EVCS.TriNM.Repositories.Models;

public partial class StationTriNm
{
    public int StationTriNmid { get; set; }

    public string StationTriNmname { get; set; }

    public bool IsActive { get; set; }

    public string StationTriNmcode { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string Province { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int Capacity { get; set; }

    public int CurrentAvailable { get; set; }

    public string Owner { get; set; }

    public string ContactPhone { get; set; }

    public string ContactEmail { get; set; }

    public string Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string ImageUrl { get; set; }

    public virtual ICollection<ChargerTriNm> ChargerTriNms { get; set; } = new List<ChargerTriNm>();
}
