using System;

namespace eShop.Loader
{
    public sealed class ProductStorage
    {
        public int ProductNo { get; set; }
        public Nullable<short> Storage { get; set; }

        public ProductMain ProductMain { get; set; }
    }
}