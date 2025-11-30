#nullable disable
using System;
using System.Collections.Generic;

namespace EVCS.TriNM.Repositories.Models;

public partial class ChargingTransaction
{
    public long TransactionId { get; set; }

    public int UserAccountId { get; set; }

    public int ChargerId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? EnergyConsumed { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ChargerTriNm ChargerTriNm { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual UserAccount UserAccount { get; set; }
}
