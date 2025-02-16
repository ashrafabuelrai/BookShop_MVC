using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            Product old = _db.products.FirstOrDefault(p => p.Id == product.Id);
            if(old!=null)
            {
                old.Title = product.Title;
                old.Description = product.Description;
                old.CategoryId = product.CategoryId;
                old.ListPrice = product.ListPrice;
                old.Price = product.Price;
                old.Author = product.Author;
                old.Price100 = product.Price100;
                old.Price50 = product.Price50;
                old.ProductImages = product.ProductImages;
                //if(product.ImageUrl!=null)
                //{
                //    old.ImageUrl = product.ImageUrl;
                //}
            }
        }
    }
}