using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repostiory
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {

        private readonly ApplicationDbContext _db;

        public ProductImageRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(ProductImage productImage)
        {
            _db.Update(productImage);
        }
    }
}
