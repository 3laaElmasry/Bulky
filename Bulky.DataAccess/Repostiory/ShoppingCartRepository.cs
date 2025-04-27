

using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repostiory
{
    public class ShoppingCartRepository : Repository<ShoppingCart>,IShoppingCartRepository
    {

        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _db.Update(shoppingCart);
        }
    }
}
