

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepo { get; }

        IProductRepository ProductRepo { get; }

        ICompanyRepository CompanyRepo { get; }

        Task Save();
    }
}
