

using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
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
            return _db.Products.Include(c => c.Category).ToList();
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
            _db.Products.Update(obj);
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                if (obj.ImgUrl != null)
                {
                    objFromDb.ImgUrl = obj.ImgUrl;
                }

            }
        }
        
    }
}
