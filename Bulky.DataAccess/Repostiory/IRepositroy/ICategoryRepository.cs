using Bulky.Models;

namespace Bulky.DataAccess.Repostiory.IRepositroy
{
    public interface ICategoryRepository : IRepository<Category>
    {

        void Upda(Category obj);

        void Save();
    }
}
