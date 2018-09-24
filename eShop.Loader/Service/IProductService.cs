using System.Collections.Generic;

namespace eShop.Loader.Service
{
    public interface IProductService
    {
        int GetStorage(string schema);

        IEnumerable<ProductMain> GetProductList();
    }
}
