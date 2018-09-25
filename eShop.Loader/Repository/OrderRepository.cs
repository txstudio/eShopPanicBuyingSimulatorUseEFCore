using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace eShop.Loader
{
    public class OrderRepository : IOrderRepository, IDisposable
    {
        private eShopContext _context;

        public OrderRepository(eShopContext context)
        {
            _context = context;
        }

        public void InsertOrderMain(OrderMain orderMain)
        {
            this._context.OrderMains.Add(orderMain);
        }

        public void InsertOrderDetail(OrderDetail orderDetail)
        {
            this._context.OrderDetails.Add(orderDetail);
        }

        //Orders.OrderMainSeq
        public int GetOrderMainSeq()
        {
            return this.GetCurrentSequenceValue("Orders.OrderMainSeq");
        }

        //Orders.OrderSchemaSeq
        public int GetOrderSchemaSeq()
        {
            return this.GetCurrentSequenceValue("Orders.OrderSchemaSeq");
        }

        /// <summary>取得指定 Sequence 名稱的數值</summary>
        private int GetCurrentSequenceValue(string sequenceName, IDbContextTransaction transaction = null)
        {
            using (var _cmd = this._context.Database.GetDbConnection().CreateCommand())
            {
                _cmd.CommandText = $"SELECT NEXT VALUE FOR {sequenceName}";

                if (transaction != null)
                    _cmd.Transaction = transaction.GetDbTransaction();

                this._context.Database.OpenConnection();
                var _result = _cmd.ExecuteScalar();
                this._context.Database.CloseConnection();

                return Convert.ToInt32(_result);
            }
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
