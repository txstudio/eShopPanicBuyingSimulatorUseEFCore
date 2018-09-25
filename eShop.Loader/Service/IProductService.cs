using System.Collections.Generic;

namespace eShop.Loader
{
    public interface IProductService
    {
        int GetStorage(string schema);

        IEnumerable<ProductMain> GetProductList();
    }
}
