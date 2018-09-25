using System.Collections.Generic;

namespace eShop.Loader
{
    public interface IProductRepository
    {
        IEnumerable<ProductMain> GetProductList();

        ProductMain GetProductById(int ProductNo);

        ProductMain GetProductBySchema(string schema);

        IEnumerable<ProductStorage> GetProductStorageList();

        ProductStorage GetProductStorageById(int ProductNo);
    }
}
