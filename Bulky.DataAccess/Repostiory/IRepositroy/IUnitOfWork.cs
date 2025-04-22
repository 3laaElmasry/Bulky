

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        void Save();
    }
}
