namespace PhoneSaleAPI.DTO
{
    public class UpdateAmountRequest
    {
        public string ShoppingCartId { get; set; }
        public string ProductId { get; set; }
        public int NewAmount { get; set; }
    }

}
