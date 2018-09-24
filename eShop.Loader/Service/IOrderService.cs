using System;
using System.Collections.Generic;

namespace eShop.Loader.Service
{
    public interface IOrderService
    {
        bool AddOrder(Guid memberGUID, List<OrderDetail> items);
    }
}
