using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ProductReview
    {
        public ProductReview()
        {
            ReviewImages = new HashSet<ReviewImage>();
        }

        public string ProductReviewId { get; set; } = null!;
        public string? Title { get; set; }
        public string? ProductId { get; set; }
        public string? CustomerId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ICollection<ReviewImage> ReviewImages { get; set; }
    }
}
