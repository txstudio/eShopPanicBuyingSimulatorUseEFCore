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

        //修改型別為 ICollection 確認是否可以接受
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
