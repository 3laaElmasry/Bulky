

using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        public void Update(ShoppingCart shoppingCart);
    }
}
