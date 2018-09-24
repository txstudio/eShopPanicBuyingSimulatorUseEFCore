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

        public bool AddOrder(Guid memberGUID, List<OrderDetail> items)
        {
            using (var _transaction
                    = this._context.Database.BeginTransaction())
            {
                try
                {
                    //購買商品是否還有足夠的庫存
                    foreach (var _item in items)
                    {
                        var _match = this._context.ProductStorages
                                                    .Where(x => x.ProductNo == _item.ProductNo)
                                                    .FirstOrDefault();

                        _match.Storage = Convert.ToInt16(_match.Storage - _item.Quantity);

                        if (_match.Storage < 0)
                        {
                            _transaction.Rollback();

                            return false;
                        }
                    }

                    string _orderSchema;

                    OrderMain _orderMain;
                    OrderDetail _orderDetail;
                    int _identity;

                    //建立商品訂單編號
                    //_orderSchema = String.Format("{0:yyyyMMdd}{1,7}"
                    //                            , DateTime.Now
                    //                            , this.GetOrderSchemaSeq(_transaction));
                    //_orderSchema = _orderSchema.Replace(" ", "0");

                    //_identity = this.GetOrderMainSeq(_transaction);

                    //新增購買商品訂單
                    _orderMain = new OrderMain();
                    _orderMain.No = _identity;
                    _orderMain.Schema = _orderSchema;
                    _orderMain.OrderDate = DateTime.Now;
                    _orderMain.MemberGUID = memberGUID;
                    _orderMain.IsDeleted = false;

                    this._context.OrderMains.Add(_orderMain);

                    foreach (var item in items)
                    {
                        _orderDetail = new OrderDetail();
                        _orderDetail.OrderNo = _orderMain.No;
                        _orderDetail.ProductNo = item.ProductNo;
                        _orderDetail.SellPrice = item.SellPrice;
                        _orderDetail.Quantity = item.Quantity;

                        this._context.OrderDetails.Add(_orderDetail);
                    }


                    this._context.SaveChanges();

                    _transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();

                    return false;
                }
            }
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
