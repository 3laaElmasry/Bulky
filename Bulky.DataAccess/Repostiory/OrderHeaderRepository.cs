
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repostiory
{
    public class OrderHeaderRepository : Repository<OrderHeader> ,IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public async Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);
            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public async Task UpdateStripePaymentId(int id, string sessionId, string paymentIntendId)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);

            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }

                if (!string.IsNullOrEmpty(paymentIntendId))
                {
                    orderFromDb.PaymentIntentedId = paymentIntendId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
