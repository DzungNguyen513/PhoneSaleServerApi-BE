﻿namespace PhoneSaleAPI.DTO
{
    public class AdminCreateProduct
    {
        public string ProductName { get; set; }
        public int StorageGB { get; set; }
        public string ColorName { get; set; }
        public int? Amount { get; set; }
        public int? Price { get; set; }
        public int? Discount { get; set; }
        public string CategoryId { get; set; }
        public string VendorId { get; set; }
        public string Detail { get; set; }
        public int? Status { get; set; }
    }
}
