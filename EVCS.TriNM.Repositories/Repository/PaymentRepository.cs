using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;

namespace EVCS.TriNM.Repositories.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(EVChargingDBContext context) : base(context)
        {
        }
    }

    public interface IPaymentRepository : IGenericRepository<Payment>
    {
    }
}
