

using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repostiory
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Add(Product entity)
        {
            _db.Products.Add(entity);

        }

        public Product? Get(Expression<Func<Product, bool>> filter)
        {
            return _db.Products.FirstOrDefault(filter);
        }

        public IEnumerable<Product> GetAll()
        {
            return _db.Products.ToList();
        }

        public void Remove(Product entity)
        {
            _db.Remove(entity);

        }

        public void RemoveRange(IEnumerable<Product> entities)
        {
            _db.RemoveRange(entities);

        }

        public void Update(Product obj)
        {
            _db.Update(obj);

        }

        
    }
}
