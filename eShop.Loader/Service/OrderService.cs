using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Loader.Service
{
    public sealed class OrderService : IOrderService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public bool AddOrder(Guid memberGUID, List<OrderDetail> items)
        {
            try
            {
                var _productInstance = this._unitOfWork.ProductRepository;
                var _orderInstance = this._unitOfWork.OrderRepository;

                //購買商品是否還有足夠的庫存
                bool _hasStorage = false;

                IEnumerable<ProductMain> _products;
                ProductStorage _storage;

                _products = _productInstance.GetProductList();

                foreach (var item in _products)
                {
                    _storage = _productInstance.GetProductStorageById(item.No);

                    if (_storage.Storage > 0)
                        _hasStorage = true;

                    if (_hasStorage == true)
                        break;
                }

                if (_hasStorage == false)
                    return false;

                string _orderSchema;
                int _schemaSeq;
                int _mainSeq;

                OrderMain _orderMain;
                OrderDetail _orderDetail;

                //建立商品訂單編號
                _schemaSeq = _orderInstance.GetOrderSchemaSeq();
                _mainSeq = _orderInstance.GetOrderMainSeq();

                _orderSchema = String.Format("{0:yyyyMMdd}{1,7}"
                                            , DateTime.Now
                                            , _schemaSeq);
                _orderSchema = _orderSchema.Replace(" ", "0");

                _mainSeq = this._unitOfWork.OrderRepository.GetOrderMainSeq();

                //新增購買商品訂單
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

                this._unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
