using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface IProductRepository : IRepository<Product>
    {
         void Update(Product obj);
    }
}
