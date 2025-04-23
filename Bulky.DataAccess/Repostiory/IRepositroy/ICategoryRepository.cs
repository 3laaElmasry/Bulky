using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface ICategoryRepository : IRepository<Category>
    {

        void Update(Category obj);

        
    }
}
