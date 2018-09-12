using System;
using System.Collections.Generic;

namespace eShop.Loader
{
    public sealed class ProductMain
    {
        public int No { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> SellPrice { get; set; }
        
        public ProductStorage ProductStorage { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}