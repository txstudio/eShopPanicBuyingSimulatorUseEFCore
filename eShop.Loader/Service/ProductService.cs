using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Loader.Service
{
    public sealed class ProductService : IProductService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IEnumerable<ProductMain> GetProductList()
        {
            return this._unitOfWork.ProductRepository.GetProductList();
        }

        public int GetStorage(string schema)
        {
            var _instance = this._unitOfWork.ProductRepository;

            var _product = _instance.GetProductBySchema(schema);
            var _storage = _instance.GetProductStorageById(_product.No);

            return Convert.ToInt32(_storage.Storage);
        }
    }
}
