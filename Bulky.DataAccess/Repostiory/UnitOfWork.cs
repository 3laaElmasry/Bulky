using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repostiory.IRepositroy;

namespace BulkyBook.DataAccess.Repostiory
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set;}


        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
        }

        public void Save()
        {
            Category.Save();
        }
    }
}
