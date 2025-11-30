#nullable disable
using System;
using System.Collections.Generic;

namespace EVCS.TriNM.Repositories.Models;

public partial class ChargerTriNm
{
    public int ChargerTriNmid { get; set; }

    public int StationTriNmid { get; set; }

    public string ChargerTriNmtype { get; set; }

    public bool IsAvailable { get; set; }

    public string ImageUrl { get; set; }

    public virtual StationTriNm StationTriNm { get; set; }

    public virtual ICollection<ChargingTransaction> ChargingTransactions { get; set; } = new List<ChargingTransaction>();
}
