using System;
using System.Collections.Generic;

namespace eShop.Loader
{
    public interface IOrderRepository
    {
        void InsertOrderMain(OrderMain orderMain);

        void InsertOrderDetail(OrderDetail orderDetail);

        int GetOrderMainSeq();

        int GetOrderSchemaSeq();
    }
}
