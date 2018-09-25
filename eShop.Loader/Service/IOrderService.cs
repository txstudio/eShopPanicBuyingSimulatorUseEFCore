using System;
using System.Collections.Generic;

namespace eShop.Loader
{
    public interface IOrderService
    {
        bool AddOrder(Guid memberGUID, List<OrderDetail> items);
    }
}
