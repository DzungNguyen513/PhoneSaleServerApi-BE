namespace PhoneSaleAPI.DTO.ShoppingCart
{
    public class CartDetailDTO
    {
        public string ShoppingCartId { get; set; } 
        public string ProductId { get; set; } 
        public string ColorName { get; set; } 
        public int StorageGb { get; set; }
        public int? Amount { get; set; }
        public int? Total { get; set; }
    }
}
