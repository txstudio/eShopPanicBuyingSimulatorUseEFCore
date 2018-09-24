using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eShop.Loader
{
    public class ProductRepository : IProductRepository, IDisposable
    {
        private eShopContext _context;

        public ProductRepository(eShopContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductMain> GetProductList()
        {
            return this._context.ProductMains.ToList();
        }

        public ProductMain GetProductBySchema(string schema)
        {
            return this._context.ProductMains
                                .Where(x => x.Schema == schema)
                                .FirstOrDefault();
        }

        public IEnumerable<ProductStorage> GetProductStorageList()
        {
            return this._context.ProductStorages.ToList();
        }

        public ProductStorage GetProductStorageById(int ProductNo)
        {
            return this._context.ProductStorages.Find(ProductNo);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing == true)
                {
                    this._context.Dispose();
                }
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
