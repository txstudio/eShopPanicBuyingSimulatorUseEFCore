using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Loader
{
    public class UnitOfWork : IDisposable
    {
        private eShopContext _context;

        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;
        private IEventLogRepository _eventLogRepository;

        public UnitOfWork()
        {
            this._context = new eShopContext();
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                if (this._orderRepository == null)
                    this._orderRepository = new OrderRepository(this._context);

                return this._orderRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                if (this._productRepository == null)
                    this._productRepository = new ProductRepository(this._context);

                return this._productRepository;
            }
        }

        public IEventLogRepository EventLogRepository
        {
            get
            {
                if (this._eventLogRepository == null)
                    this._eventLogRepository = new EventLogRepository(this._context);

                return this._eventLogRepository;
            }
        }

        public void Save()
        {
            this._context.SaveChanges();
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
