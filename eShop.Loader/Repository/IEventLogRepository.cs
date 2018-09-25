using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Loader
{
    public interface IEventLogRepository
    {
        void InsertEventLog(EventLog eventLog);
    }
}
