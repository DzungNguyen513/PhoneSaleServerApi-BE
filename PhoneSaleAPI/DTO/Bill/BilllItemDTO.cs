namespace PhoneSaleAPI.DTO.Bill
{
    public class BillSummaryDTO
    {
        public string BillId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public string Note { get; set; }
        public List<BillItemDto> lstProductBill { get; set; }
    }
    public class BillItemDto
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int OriginalPrice { get; set; }
        public int DiscountedPrice { get; set; }
        public string ColorName { get; set; }
        public int StorageGB { get; set; }
        public int Amount { get; set; }
        public string Img { get; set; }
    }

}
