using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace eShop.Loader
{
    public sealed class OrderService : IOrderService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        private StringComparison _comparison = StringComparison.OrdinalIgnoreCase;

        public bool AddOrder(Guid memberGUID, List<OrderDetail> items)
        {
            bool _IsSaveOK = false;
            bool _AddOrderResult = false;
            bool _hasStorage = false;

            var _log = new EventLog();
            var _productInstance = this._unitOfWork.ProductRepository;
            var _orderInstance = this._unitOfWork.OrderRepository;
            var _stopwatch = new Stopwatch();

            _log.MemberGUID = memberGUID;
            _log.Retry = 0;
            _log.EventDateTime = DateTime.Now;

            //判斷商品庫存：沒有庫存的話就不進行新增訂單
            ProductStorage _productStorage;
            ProductMain _productMain;

            _stopwatch.Reset();
            _stopwatch.Start();

            foreach (var item in items)
            {
                _productStorage = _productInstance.GetProductStorageById(item.ProductNo);
                _productMain = _productInstance.GetProductById(item.ProductNo);

                _log.ProductName = _productMain.Name;
                _log.ProductSchema = _productMain.Schema;
                _log.Quantity = item.Quantity;
                _log.OrginalStorage = _productStorage.Storage;

                _productStorage.Storage = Convert.ToInt16(_productStorage.Storage.Value - item.Quantity.Value);

                if (_productStorage.Storage >= 0)
                    _hasStorage = true;

                if (_hasStorage == true)
                    break;
            }

            if (_hasStorage == false)
                return false;

            //取得訂單序號與訂單編號
            int _mainSeq, _schemaSeq;
            string _orderSchema;

            _mainSeq = _orderInstance.GetOrderMainSeq();
            _schemaSeq = _orderInstance.GetOrderSchemaSeq();

            _orderSchema = String.Format("{0:yyyyMMdd}{1,7}", DateTime.Now, _schemaSeq);
            _orderSchema = _orderSchema.Replace(" ", "0");

            //建立訂單
            OrderMain _orderMain;
            OrderDetail _orderDetail;

            _orderMain = new OrderMain();
            _orderMain.No = _mainSeq;
            _orderMain.Schema = _orderSchema;
            _orderMain.OrderDate = DateTime.Now;
            _orderMain.MemberGUID = memberGUID;
            _orderMain.IsDeleted = false;

            this._unitOfWork.OrderRepository.InsertOrderMain(_orderMain);

            foreach (var item in items)
            {
                _orderDetail = new OrderDetail();
                _orderDetail.OrderNo = _mainSeq;
                _orderDetail.ProductNo = item.ProductNo;
                _orderDetail.SellPrice = item.SellPrice;
                _orderDetail.Quantity = item.Quantity;

                this._unitOfWork.OrderRepository.InsertOrderDetail(_orderDetail);
            }

            /*
             * 此區塊程式碼參考至下列 Github
             * https://github.com/aspnet/EntityFramework.Docs/blob/master/samples/core/Saving/Saving/Concurrency/Sample.cs
             */
            while (_IsSaveOK == false)
            {
                try
                {
                    _log.Retry = (_log.Retry + 1);

                    this._unitOfWork.Save();

                    _IsSaveOK = true;

                    _AddOrderResult = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    int _ProductNo = (-1);

                    //取得 ProductStorage 庫存物件清單並進行更新
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is ProductStorage)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues[property];

                                if (string.Equals("ProductNo", property.Name, _comparison) == true)
                                {
                                    _ProductNo = Convert.ToInt32(proposedValue);
                                }

                                if (string.Equals("Storage", property.Name, _comparison) == true)
                                {
                                    short _storage;
                                    short _storageUpdated;

                                    //取得要購買的庫存量
                                    var _match = items.Where(x => x.ProductNo == _ProductNo).FirstOrDefault();

                                    if (_match == null)
                                    {
                                        _IsSaveOK = true;
                                        break;
                                    }

                                    //重新取得當下資料庫儲存的庫存量並進行更新
                                    _storage = Convert.ToInt16(databaseValue);
                                    _storageUpdated = Convert.ToInt16(_storage - _match.Quantity);

                                    if (_storageUpdated >= 0)
                                    {
                                        proposedValues[property] = _storageUpdated;
                                        databaseValues[property] = _storage;
                                    }
                                    else
                                    {
                                        _IsSaveOK = true;
                                        break;
                                    }
                                }
                            }

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType().Name);

                    break;
                }
            }

            _stopwatch.Stop();

            _log.Elapsed = Convert.ToInt32(_stopwatch.ElapsedMilliseconds);
            _log.IsSuccess = _AddOrderResult;

            //儲存事件紀錄
            using (UnitOfWork _work = new UnitOfWork())
            {
                _work.EventLogRepository.InsertEventLog(_log);
                _work.Save();
            }

            return _AddOrderResult;
        }

    }
}
