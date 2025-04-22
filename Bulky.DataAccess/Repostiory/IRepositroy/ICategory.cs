using Bulky.Models;

namespace Bulky.DataAccess.Repostiory.IRepositroy
{
    public interface ICategory : IRepository<Category>
    {

        void Upda(Category obj);

        void Save();
    }
}
