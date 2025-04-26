using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repostiory.IRepositroy;

namespace BulkyBook.DataAccess.Repostiory
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepo { get; private set;}

        public IProductRepository ProductRepo { get; private set;}

        public ICompanyRepository CompanyRepo { get; private set;}

        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepo = new CategoryRepository(_db);
            ProductRepo = new ProductRepository(_db);
            CompanyRepo = new CompanyRepository(_db);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
