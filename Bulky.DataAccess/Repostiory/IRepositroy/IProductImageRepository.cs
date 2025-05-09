
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repostiory.IRepositroy
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        public void Update(ProductImage productImage);
    }
}
