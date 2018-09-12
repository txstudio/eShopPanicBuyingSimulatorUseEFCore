using System;
using System.Collections.Generic;

namespace eShop.Loader
{
    public sealed class OrderMain
    {
        public int No { get; set; }
        public string Schema { get; set; }
        public Nullable<DateTimeOffset> OrderDate { get; set; }
        public Nullable<Guid> MemberGUID { get; set; }
        public bool? IsDeleted { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}