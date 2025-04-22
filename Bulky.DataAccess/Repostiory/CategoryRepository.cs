using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repostiory.IRepositroy;
using Bulky.Models;
using System.Linq.Expressions;


namespace Bulky.DataAccess.Repostiory
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

        public void Save()
        {
           _db.SaveChanges();
        }

        public void Upda(Category obj)
        {
            _db.Update(obj);
            
        }
    }
}
