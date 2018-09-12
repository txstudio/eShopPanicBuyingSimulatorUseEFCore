using System;
using System.Collections.Generic;

namespace eShop.Loader
{
    public interface IShopRepository
    {
        IEnumerable<ProductMain> GetProducts();

        int GetProductValidStorage(string schema);

        bool AddOrder(Guid memberGUID, List<OrderDetail> items);
    }
}