using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using System.Linq.Expressions;


namespace BulkyBook.DataAccess.Repostiory
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Add(Category entity)
        {
            _db.Categories.Add(entity);
            
        }

        public Category? Get(Expression<Func<Category, bool>> filter)
        {
            return _db.Categories.FirstOrDefault(filter);
        }

        public IEnumerable<Category> GetAll()
        {
            return _db.Categories.ToList();
        }

        public void Remove(Category entity)
        {
            _db.Remove(entity);
            
        }

        public void RemoveRange(IEnumerable<Category> entities)
        {
            _db.RemoveRange(entities);
            
        }

       

        public void Update(Category obj)
        {
            _db.Update(obj);
            
        }
    }
}
