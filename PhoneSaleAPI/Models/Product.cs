using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Product
    {
        public Product()
        {
            BillDetails = new HashSet<BillDetail>();
            ProductDetails = new HashSet<ProductDetail>();
            ProductImages = new HashSet<ProductImage>();
            ProductReviews = new HashSet<ProductReview>();
            ShoppingCartDetails = new HashSet<ShoppingCartDetail>();
        }

        public string ProductId { get; set; } = null!;
        public string? ProductName { get; set; }
<<<<<<< HEAD
=======
        public int? Amount { get; set; }
>>>>>>> 1f5b15b24183bf0102df0552a617a4d6d35ad5e1
        public int? Price { get; set; }
        public int? Discount { get; set; }
        public string? CategoryId { get; set; }
        public string? VendorId { get; set; }
        public string? Detail { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Vendor? Vendor { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<ShoppingCartDetail> ShoppingCartDetails { get; set; }
    }
}
