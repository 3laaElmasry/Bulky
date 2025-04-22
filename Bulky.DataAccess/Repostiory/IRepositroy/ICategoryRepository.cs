using Bulky.Models;

namespace Bulky.DataAccess.Repostiory.IRepositroy
{
    public interface ICategoryRepository : IRepository<Category>
    {

        void Update(Category obj);

        void Save();
    }
}
