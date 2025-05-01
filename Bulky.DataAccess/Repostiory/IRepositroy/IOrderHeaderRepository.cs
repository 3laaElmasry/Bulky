
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System.Runtime.CompilerServices;

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);

        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        Task UpdateStripePaymentId(int id, string sessionId, string paymentIntendId);
    }
}
