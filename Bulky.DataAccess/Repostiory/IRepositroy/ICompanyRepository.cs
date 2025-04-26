using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface ICompanyRepository : IRepository<Company>
    {
        public void Update(Company company);

    }
}
