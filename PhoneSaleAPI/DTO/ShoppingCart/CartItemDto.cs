﻿namespace PhoneSaleAPI.DTO.ShoppingCart
{
    public class CartItemDto
    {
        public string ShoppingCartId { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int OriginalPrice { get; set; }
        public int DiscountedPrice { get; set; }
        public string ColorName { get; set; }
        public int StorageGB { get; set; }
        public int Amount { get; set; }
        public string? Img { get; set; }
    }
}
