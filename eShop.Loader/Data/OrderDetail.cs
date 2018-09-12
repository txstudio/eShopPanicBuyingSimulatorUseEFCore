using System;

namespace eShop.Loader
{
    public sealed class OrderDetail
    {
        public int OrderNo { get; set; }
        public int ProductNo { get; set; }
        public Nullable<decimal> SellPrice { get; set; }
        public Nullable<short> Quantity { get; set; }

        public OrderMain OrderMain { get; set; }
        public ProductMain ProductMain { get; set; }
    }
}