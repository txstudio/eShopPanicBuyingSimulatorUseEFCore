using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Loader
{
    public sealed class EventLog
    {
        public int No { get; set; }
        public Nullable<DateTimeOffset> EventDateTime { get; set; }

        public Nullable<Guid> MemberGUID { get; set; }
        public string ProductSchema { get; set; }
        public string ProductName { get; set; }

        public short? OrginalStorage { get; set; }
        public short? Quantity { get; set; }

        public int? Elapsed { get; set; }
        public bool? IsSuccess { get; set; }

        public string Exception { get; set; }
        public int? Retry { get; set; }
    }
}
