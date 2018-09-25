using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Loader
{
    public class EventLogRepository : IEventLogRepository, IDisposable
    {
        private eShopContext _context;

        public EventLogRepository(eShopContext context)
        {
            _context = context;
        }

        public void InsertEventLog(EventLog eventLog)
        {
            this._context.EventLogs.Add(eventLog);
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
