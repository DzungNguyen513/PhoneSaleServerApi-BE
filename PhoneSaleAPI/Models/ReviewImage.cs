using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ReviewImage
    {
        public string ReviewImageId { get; set; } = null!;
        public string? ProductReviewId { get; set; }
        public string? ImagePath { get; set; }
        public bool? IsPrimary { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ProductReview? ProductReview { get; set; }
    }
}
